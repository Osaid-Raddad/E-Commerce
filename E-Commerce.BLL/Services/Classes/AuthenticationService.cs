using E_Commerce.BLL.Services.Interfaces;
using E_Commerce.DAL.DTO.Request;
using E_Commerce.DAL.DTO.Response;
using E_Commerce.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services.Classes
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            var user = registerRequest.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();

                return new RegisterResponse
                {
                    Success = false,
                    Message = string.Join(", ", errors)
                };
            }

            await _userManager.AddToRoleAsync(user, "User");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = Uri.EscapeDataString(token);

            var emailUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/Account/ConfirmEmail?token={token}&userId={user.Id}";

            await _emailSender.SendEmailAsync(
                                user.Email,
                                "Confirm Your Email - E-Commerce",
                                $@"<!DOCTYPE html>
             <html lang=""en"">
             <head>
             <meta charset=""UTF-8"">
             <title>Confirm Email</title>
             </head>

             <body style=""margin:0;padding:0;background:#f2f5f9;font-family:Arial,Helvetica,sans-serif;"">

             <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
             <tr>
             <td align=""center"">

             <table width=""600"" cellpadding=""0"" cellspacing=""0""
             style=""background:white;border-radius:10px;overflow:hidden;margin-top:40px;box-shadow:0 5px 20px rgba(0,0,0,0.08);"">

             <tr>
             <td style=""background:linear-gradient(135deg,#4f46e5,#3b82f6);padding:30px;text-align:center;color:white;"">
             <h1 style=""margin:0;font-size:26px;"">E-Commerce</h1>
             <p style=""margin-top:8px;font-size:14px;opacity:0.9;"">Welcome to our platform</p>
             </td>
             </tr>

             <tr>
             <td style=""padding:40px;text-align:center;"">

             <h2 style=""margin-top:0;color:#333;"">Confirm Your Email</h2>

             <p style=""font-size:16px;color:#555;line-height:1.6;"">
             Hello <strong>{user.UserName}</strong>,<br><br>
             Thank you for creating an account with us.<br>
             Please confirm your email address to activate your account.
             </p>

             <div style=""margin:35px 0;"">
             <a href=""{emailUrl}""
             style=""background:#4f46e5;
             color:white;
             padding:14px 35px;
             font-size:16px;
             text-decoration:none;
             border-radius:6px;
             display:inline-block;
             font-weight:bold;"">
             Confirm Email
             </a>
             </div>

             <p style=""font-size:14px;color:#777;"">
             If you didn't create this account, you can safely ignore this email.
             </p>

             </td>
             </tr>

             <tr>
             <td style=""background:#f9fafc;padding:20px;text-align:center;font-size:12px;color:#888;"">
             © 2026 E-Commerce Platform<br>
             All rights reserved
             </td>
             </tr>

             </table>

             </td>
             </tr>
             </table>

             </body>
             </html>"
);

            return new RegisterResponse
            {
                Message = "User registered successfully",
                Success = true
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user is null)
                return new LoginResponse() { Message = "Invalid Email", Success = false };
            
            if(!await _userManager.IsEmailConfirmedAsync(user))
                return new LoginResponse() { Message = "Email is not confirmed", Success = false };

            var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!result)
                return new LoginResponse() { Message = "Invalid password", Success = false };

            return new LoginResponse() { Message = "Login successful", Success = true , AccessToken = await GenerateAccessToken(user) };
        }

        public async Task<bool> ConfirmEmailAsync(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if(!result.Succeeded) return false;

            return true;
        }

        private async Task<string> GenerateAccessToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(5),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
