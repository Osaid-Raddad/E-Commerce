using E_Commerce.DAL.DTO.Request;
using E_Commerce.DAL.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest);
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        Task<bool> ConfirmEmailAsync(string token, string userId);
        Task<ForgotPasswordResponse> RequestForgotPassAsync(ForgotPasswordRequest forgotPasswordRequest);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest);
    }
}
