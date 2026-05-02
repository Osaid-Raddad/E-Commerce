using E_Commerce.BLL.Services.Interfaces;
using E_Commerce.DAL.DTO.Request;
using E_Commerce.DAL.DTO.Response;
using E_Commerce.DAL.Models;
using E_Commerce.DAL.Repository.Classes;
using E_Commerce.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using Stripe.Checkout;


namespace E_Commerce.BLL.Services.Classes
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;
        private readonly IProductRepository _productRepository;
        private readonly Interfaces.IEmailSender _emailSender;

        public CheckoutService(ICartRepository cartRepository,
            UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor,
            IOrderRepository orderRepository, ICartService cartService, IProductRepository productRepository,
             Interfaces.IEmailSender emailSender) 
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _orderRepository = orderRepository;
            _cartService = cartService;
            _productRepository = productRepository;
            _emailSender = emailSender;
        }

        public async Task<CheckoutResponse> ProcessCheckout(string userId, CheckoutRequest request)
        {
            var cartItems = await _cartRepository.GetAllAsync(
                filter: c => c.UserId == userId,
                includes: new[] {
                    nameof(Cart.Product),
                    $"{nameof(Cart.Product)}.{nameof(DAL.Models.Product.Translations)}"
                }
            );

            if (!cartItems.Any())
                return new CheckoutResponse
                {
                    Success = false,
                    Error = "cart is empty"
                };
            var user = await _userManager.FindByIdAsync(userId);

            var city = request.City ?? user.City;
            if (city is null)
                return new CheckoutResponse
                {
                    Success = false,
                    Error = "city is required"
                };

            var street = request.Street ?? user.Street;
            if (street is null)
                return new CheckoutResponse
                {
                    Success = false,
                    Error = "street is required"
                };

            var phoneNumber = request.PhoneNumber ?? user.City;
            if (city is null)
                return new CheckoutResponse
                {
                    Success = false,
                    Error = "city is required"
                };

            foreach (var item in cartItems)
            {
                if (item.Count > item.Product.Quantity)
                {
                    return new CheckoutResponse
                    {
                        Success = false,
                        Error = "dosn't have enough stock"
                    };
                }
            }
            var order = new Order()
            {
                UserId = userId,
                City = city,
                Street = street,
                PhoneNumber = phoneNumber,
                PaymentMethod = request.PaymentMethod,
                AmoundPaid = cartItems.Sum(c => c.Product.Price * c.Count),
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Count,
                    UnitPrice = c.Product.Price,
                    TotalPrice = c.Product.Price * c.Count,
                }).ToList()
            };
            await _orderRepository.CreateAsync(order);

            if (request.PaymentMethod == PaymentMethodEnum.Cash)
            {
                return new CheckoutResponse
                {
                    Success = true,
                };
            }

            if (request.PaymentMethod == PaymentMethodEnum.Visa)
            {
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    Mode = "payment",
                    SuccessUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/checkout/success?sessionId={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/checkout/cancel",
                    LineItems = new List<SessionLineItemOptions>()
                };

                foreach (var item in cartItems)
                {
                    options.LineItems.Add(
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "USD",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Product.Translations.FirstOrDefault(t => t.Language == "en").Name,
                                },
                                UnitAmount = (long)item.Product.Price,
                            },
                            Quantity = item.Count,
                        }
                    );
                }
                var service = new SessionService();
                var session = service.Create(options);
                order.StripeSessionId = session.Id;
                await _orderRepository.UpdateAsync(order);
                return new CheckoutResponse
                {
                    Success = true,
                };

            }
            return new CheckoutResponse
            {
                Success = false,
                Error = "invalid payment method"
            };
        }

        public async Task<CheckoutResponse> HanldeSuccess(string sessionId)
        {
            var order = await _orderRepository.GetOneAsync(
                o => o.StripeSessionId == sessionId,
                includes: new[] 
                {
                    nameof(Order.OrderItems),
                    $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}",
                    $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}.{nameof(DAL.Models.Product.Translations)}"

                }
                );

            order.OrderStatus = OrderStatusEnum.Paid;
            await _orderRepository.UpdateAsync(order);

            await _cartService.ClearCart(order.UserId);

            var user = await _userManager.FindByIdAsync(order.UserId);
            await _emailSender.SendEmailAsync(user.Email, "order confimed", "<h2> your order has beed placed succesfully</h2>");

            var lowStockProducts = await _productRepository.DecreaseQuantityAsync(order.OrderItems);
            foreach (var item in lowStockProducts)
            {
                if (lowStockProducts != null)
                {
                    await _emailSender.SendEmailAsync($"tariq@eknowledge.ps", "low stock alert",
                    $"<h2>product {item.Translations.FirstOrDefault(t => t.Language == "en").Name} current quantity : {item.Quantity}</h2>");
                }
            }

            return new CheckoutResponse()
            {
                Success = true,
                OrderId = order.Id
            };
        }
    }
}
