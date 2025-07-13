using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using QuizPlatform.Core.Common;
using QuizPlatform.Core.DTOs;
using QuizPlatform.Core.Entities;
using QuizPlatform.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuizPlatform.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GenerateJwtToken(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name)
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<ApiResponse> Register(RegisterDto model)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return new ApiResponse(400, "This account already exists.");
                }

                var user = new User
                {
                    UserName = model.Email.Split('@')[0],
                    Email = model.Email.ToLower().Trim(),
                    Name=model.Name
                };

                var createResult = await _userManager.CreateAsync(user, model.Password);
                if (!createResult.Succeeded)
                {
                    return new ApiResponse(400, "Failed to create user.", createResult.Errors.Select(e => e.Description));
                }

                var addToRoleResult = await _userManager.AddToRoleAsync(user, "User".ToLower());
                if (!addToRoleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return new ApiResponse(400, "Failed to assign role to user.", addToRoleResult.Errors.Select(e => e.Description));
                }

                return new ApiResponse(200, "Your account has been created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during registration for email: {Email}", model.Email);
                return new ApiResponse(500, "An error occurred during registration.", ex.Message);
            }
        }
        public async Task<ApiResponse> Login(LoginDto model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model?.Email) || string.IsNullOrWhiteSpace(model.Password))
                {
                    return new ApiResponse(400, "Invalid data.");
                }

                var user = await _userManager.FindByEmailAsync(model.Email.ToLower().Trim());
                if (user == null)
                {
                    return new ApiResponse(401, "Invalid email or password.");
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, false);

                if (!result.Succeeded)
                {
                    return new ApiResponse(401, "Incorrect email or password.");
                }

                var token = await GenerateJwtToken(user);

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                return new ApiResponse(200, "Login successful.", new
                {
                    Token = token,
                    Name = user.Name,
                    Role = role
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during login for email: {Email}", model?.Email);
                return new ApiResponse(500, "An unexpected error occurred.", ex.Message);
            }
        }
        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
        }


    }
}
