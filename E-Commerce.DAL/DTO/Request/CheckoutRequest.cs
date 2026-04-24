using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.DTO.Request
{
    public enum PaymentMethodEnum
    {
        Cash = 1, Visa = 2
    }

    public class CheckoutRequest
    {
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? PhoneNumber { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
    }
}
