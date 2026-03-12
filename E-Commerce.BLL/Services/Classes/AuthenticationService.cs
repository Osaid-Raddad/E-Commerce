using E_Commerce.BLL.Services.Interfaces;
using E_Commerce.DAL.DTO.Request;
using E_Commerce.DAL.DTO.Response;
using E_Commerce.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services.Classes
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticationService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
           var user = registerRequest.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                return new RegisterResponse { Message = "User registered successfully", Success = true };
            }
           
            return new RegisterResponse { Message = "Error", Success = false };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user is null)
                return new LoginResponse() { Message = "Invalid Email", Success = false };
            
            var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!result)
                return new LoginResponse() { Message = "Invalid password", Success = false };

            return new LoginResponse() { Message = "Login successful", Success = true };
        }

    }
}
