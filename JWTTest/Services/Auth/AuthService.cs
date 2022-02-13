using JWTTest.Enums;
using JWTTest.JWT;
using JWTTest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWTTest.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWTModel _jwt;
        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWTModel> jwtOption)
        {
            _userManager = userManager;
            _jwt = jwtOption.Value;
            _roleManager = roleManager;
        }

        public async Task<string> AddRoleToUser(UserRole model)
        {
            var user = await _userManager.FindByIdAsync(model.UserID);
            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid userID or role";
            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "Is already in the role";
            var result = await _userManager.AddToRoleAsync(user, model.Role);
            return result.Succeeded ? "" : "Errors : " + string.Join(",", result.Errors.Select(x => x.Description + $"({x.Code})"));
        }

        public async Task<AuthModel> GetTokenAsync(LoginModel model)
        {
            if (model is null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return new AuthModel() { Message = "Invalid data" };

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new AuthModel() { Message = "Email or password is incorrect" };

            var token = await CreateJwtToken(user);
            return new AuthModel()
            {
                Email = user.Email,
                ExpiresOn = token.ValidTo,
                IsAuthenticated = true,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserName = user.UserName,
                Message = "Token has created successfully"
            };
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (model is null)
                return new AuthModel() { Message = "No data sent" };
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel() { Message = "Email is already registered" };
            if (await _userManager.FindByNameAsync(model.UserName) is not null)
                return new AuthModel() { Message = "UserName is already registered" };

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return new AuthModel() { Message = "Errors : " + string.Join(',', result.Errors.Select(x => x.Description).ToList()) };

            await _userManager.AddToRoleAsync(user, nameof(Roles.User));
            var jwtSecurityToken = await CreateJwtToken(user);
            return new AuthModel()
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName
            };
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            //user cliams already exist
            var userClaims = await _userManager.GetClaimsAsync(user);

            //generate cliams for each role
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(x => new Claim("role", x)).ToList();

            //object that hold allclaims we get and the user identity and guid
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("uid",user.Id),
            }
            .Union(userClaims)
            .Union(roleClaims);


            var symmeticSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmeticSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
    }
}
