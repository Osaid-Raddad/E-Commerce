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
                

                return new RegisterResponse
                {
                    Success = false,
                    Message = "Error occurred during registration",
                    Errors = result.Errors.Select(e => e.Description).ToList()
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

        public async Task<ForgotPasswordResponse> RequestForgotPassAsync(ForgotPasswordRequest forgotPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordRequest.Email);
            if (user is null)
                return new ForgotPasswordResponse() { 
                    Message = "Email not found",
                    Success = false 
                };

            var random = new Random();
            var code = random.Next(1000, 9999).ToString();

            user.RestetCode = code;
            user.RestetCodeExpiration = DateTime.Now.AddMinutes(15);
            await _userManager.UpdateAsync(user);

            await _emailSender.SendEmailAsync(user.Email,
                            "Reset Your Password - E-Commerce",
                            $@"<!DOCTYPE html>
                    <html lang=""en"">
                    <head>
                    <meta charset=""UTF-8"">
                    <title>Reset Password</title>
                    </head>

                    <body style=""margin:0;padding:0;background:#f2f5f9;font-family:Arial,Helvetica,sans-serif;"">

                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                    <tr>
                    <td align=""center"">

                    <table width=""600"" cellpadding=""0"" cellspacing=""0""
                    style=""background:white;border-radius:10px;overflow:hidden;margin-top:40px;box-shadow:0 5px 20px rgba(0,0,0,0.08);"">

                    <tr>
                    <td style=""background:linear-gradient(135deg,#ef4444,#f97316);padding:30px;text-align:center;color:white;"">
                    <h1 style=""margin:0;font-size:26px;"">E-Commerce</h1>
                    <p style=""margin-top:8px;font-size:14px;opacity:0.9;"">Password Reset Request</p>
                    </td>
                    </tr>

                    <tr>
                    <td style=""padding:40px;text-align:center;"">

                    <h2 style=""margin-top:0;color:#333;"">Reset Your Password</h2>

                    <p style=""font-size:16px;color:#555;line-height:1.6;"">
                    Hello <strong>{user.UserName}</strong>,<br><br>
                    We received a request to reset your password.<br>
                    Use the verification code below to proceed:
                    </p>

                    <div style=""margin:30px 0;"">
                    <span style=""
                    display:inline-block;
                    background:#f3f4f6;
                    padding:15px 30px;
                    font-size:28px;
                    letter-spacing:5px;
                    font-weight:bold;
                    color:#111827;
                    border-radius:8px;"">
                    {code}
                    </span>
                    </div>

                    <p style=""font-size:15px;color:#555;"">
                    This code will expire at:<br>
                    <strong>{user.RestetCodeExpiration:yyyy-MM-dd hh:mm tt}</strong>
                    </p>

                    <p style=""font-size:14px;color:#777;margin-top:20px;"">
                    If you didn't request a password reset, you can safely ignore this email.
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
        return new ForgotPasswordResponse()
            {
                Message = "Reset code sent to your email",
                Success = true
            };
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (user == null)
                return new ResetPasswordResponse() 
                {
                    Message = "Email not found",
                    Success = false
                };
            else if (user.RestetCode != resetPasswordRequest.Code || user.RestetCodeExpiration < DateTime.Now)
                return new ResetPasswordResponse() 
                { 
                    Message = "Invalid or expired code", 
                    Success = false 
                };
            var isSamePassword = await _userManager.CheckPasswordAsync(user, resetPasswordRequest.NewPassword);
            if (isSamePassword)
            {
                return new ResetPasswordResponse()
                {
                    Message = "New password cannot be the same as the old password",
                    Success = false
                };
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result =await _userManager.ResetPasswordAsync(user,token , resetPasswordRequest.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return new ResetPasswordResponse()
                {
                    Message = string.Join(", ", errors),
                    Success = false
                };
            }

            await _emailSender.SendEmailAsync(
                      user.Email,
                      "Password Changed Successfully - E-Commerce",
                      $@"<!DOCTYPE html>
                <html lang=""en"">
                <head>
                <meta charset=""UTF-8"">
                <title>Password Changed</title>
                </head>

                <body style=""margin:0;padding:0;background:#f2f5f9;font-family:Arial,Helvetica,sans-serif;"">

                <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                <td align=""center"">

                <table width=""600"" cellpadding=""0"" cellspacing=""0""
                style=""background:white;border-radius:10px;overflow:hidden;margin-top:40px;box-shadow:0 5px 20px rgba(0,0,0,0.08);"">

                <tr>
                <td style=""background:linear-gradient(135deg,#10b981,#059669);padding:30px;text-align:center;color:white;"">
                <h1 style=""margin:0;font-size:26px;"">E-Commerce</h1>
                <p style=""margin-top:8px;font-size:14px;opacity:0.9;"">Security Notification</p>
                </td>
                </tr>

                <tr>
                <td style=""padding:40px;text-align:center;"">

                <h2 style=""margin-top:0;color:#333;"">Password Updated</h2>

                <p style=""font-size:16px;color:#555;line-height:1.6;"">
                Hello <strong>{user.UserName}</strong>,<br><br>
                Your password has been successfully changed.
                </p>

                <div style=""margin:25px 0;font-size:14px;color:#555;"">
                <strong>Date:</strong> {DateTime.Now:yyyy-MM-dd}<br>
                <strong>Time:</strong> {DateTime.Now:hh:mm tt}
                </div>

                <p style=""font-size:14px;color:#777;margin-top:20px;"">
                If you did not perform this action, please contact support immediately.
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

            return new ResetPasswordResponse()
            {
                Message = "Password reset successfully",
                Success = true
            };

        }
    }
}
