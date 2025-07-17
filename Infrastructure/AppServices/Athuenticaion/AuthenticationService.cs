using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Authentication;
using Application.IAppServices.Authentication;
using Application.IRepository;
using Domain.Common;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using IAuthenticationService = Application.IAppServices.Authentication.IAuthenticationService;

namespace Infrastructure.AppServices.Athuenticaion
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityAppRepository<ApplicationUser> _userRepository;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration config,
            IIdentityAppRepository<ApplicationUser> identityAppRepository
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _userRepository = identityAppRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserProfileDTO> GetAuthenticatedUser()
        {
            if (_httpContextAccessor.HttpContext is null)
                throw new Exception("User Not found");

            var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                throw new Exception("User Not found");

            ApplicationUser? user = null;
            var identifierClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier);

            if (identifierClaim != null)
            {
                user = await _userManager.FindByIdAsync(identifierClaim.Value.ToString());
            }

            if (user is null)
            {
                throw new Exception("User Not found");
            }

            if (!user.IsActive)
            {
                throw new Exception("Deactivated User");
            }

            var jwtToken = await GenerateJwtToken(user);
            return new UserProfileDTO
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Token = jwtToken,
            };
        }


        public async Task<UserProfileDTO> LoginAsync(LoginDTO loginDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(loginDto.Email);
            if (existingUser == null)
                throw new Exception("email or password not correct");

            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, loginDto.Password);
            if (!isCorrect)
                throw new Exception("email or password not correct");

            if (!existingUser.IsActive)
                throw new Exception("Deactivated User");

            await _signInManager.SignInAsync(existingUser, false);

            var jwtToken = await GenerateJwtToken(existingUser);
            return new UserProfileDTO
            {
                Id = existingUser.Id,
                Username = existingUser.UserName ?? string.Empty,
                Email = existingUser.Email ?? string.Empty,
                Token = jwtToken,
            };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<string> RegisterAsync(RegisterDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, dto.Role.ToString());
                
                return user.Id;
            }
            var errorMessages = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errorMessages}");

        }

        public async Task<bool> IsAdmin()
        {
            if (_httpContextAccessor.HttpContext is null)
                return false;

            var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                return false;

            ApplicationUser? user = null;
            var identifierClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier);

            if (identifierClaim != null && int.TryParse(identifierClaim.Value, out int userId))
            {
                user = await _userManager.FindByIdAsync(userId.ToString());
                if (user is null)
                    return false;
            }
            var isAdmin = await _userManager.IsInRoleAsync(user!, DefaultSettings.AdminRoleName);

            return isAdmin;
        }

        private async Task<TokenDTO> GenerateJwtToken(ApplicationUser user)
        {
            try
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var secret = _config["Jwt:Secret"] ?? string.Empty;
                var key = Encoding.ASCII.GetBytes(secret);
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.Username ?? string.Empty),
                    new(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Audience = _config["Jwt:Audience"],
                    Issuer = _config["Jwt:Issuer"],
                    Subject = new ClaimsIdentity(claims.ToArray()),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiryInMinutes"])),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = jwtTokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = jwtTokenHandler.WriteToken(token);

                return new TokenDTO()
                {
                    JwtToken = jwtToken,
                    Success = true,
                    Roles = userRoles,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string?> RefreshToken(string jwtToken)
        {
            string id = RetrieveUserID();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return null;

            user.RefreshToken = Guid.NewGuid().ToString();
            await _userManager.UpdateAsync(user);
            return (await GenerateJwtToken(user)).JwtToken;
        }

        public string RetrieveUserID()
        {
            string authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers.Authorization!;
            if (authorizationHeader == null)
                throw new Exception("No authorization was found in the header");
            string token = authorizationHeader.Substring("Bearer".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            Claim userIdClaim = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "userId")!;

            if (userIdClaim == null)
                throw new Exception("UserId not found in the token.");

            return userIdClaim.Value;
        }

    }
}
