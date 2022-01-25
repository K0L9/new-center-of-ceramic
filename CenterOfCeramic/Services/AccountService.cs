using AutoMapper;
using CenterOfCeramic.Data.Helpers;
using CenterOfCeramic.Data.Helpers.LordOfTheHoney.Shared.Wrapper;
using CenterOfCeramic.Interfaces;
using CenterOfCeramic.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CenterOfCeramic.Services
{
    public class AccountService
    {

        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;
        Mapper mapper;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterUserDTO, ApplicationUser>();
            });
            mapper = new Mapper(config);
        }

        public async Task<IResult> RegisterUserAsync(RegisterUserDTO registerUser)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(registerUser.Email) != null)
                    return await Result.FailAsync("Email is exist");

                var user = mapper.Map<ApplicationUser>(registerUser);
                user.UserName = registerUser.FirstName + "_" + registerUser.LastName;
                user.Id = Guid.NewGuid().ToString();
                var result = await _userManager.CreateAsync(user, registerUser.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Constants.BasicRole);
                    return await Result<string>.SuccessAsync(user.Id, $"User {user.UserName} Registered");
                }
                return await Result.FailAsync(result.Errors.ToString());
            }
            catch (Exception ex)
            {
                return await Result.FailAsync(ex.Message);
            }
        }
        public async Task<IResult> LoginUserAsync(LoginUserDTO loginUser)
        {
            try
            {
                var userInDb = await _userManager.FindByEmailAsync(loginUser.Email);
                if (userInDb == null)
                    return await Result.FailAsync("Invalid email or password");

                var passwordValid = await _userManager.CheckPasswordAsync(userInDb, loginUser.Password);
                if (!passwordValid)
                    return await Result.FailAsync("Invalid email or password");

                var claims = await GetClaimsAsync(userInDb);
                var token = GenerateJwtToken(userInDb, claims);

                TokenResponse tokenResponse = new TokenResponse(token);

                return await Result<TokenResponse>.SuccessAsync(tokenResponse);
            }
            catch (Exception ex)
            {
                return await Result.FailAsync(ex.Message);
            }
        }
        private string GenerateJwtToken(ApplicationUser user, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(2));
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }
        private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("Role", role));
                var thisRole = await _roleManager.FindByNameAsync(role);
                var allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole);
                permissionClaims.AddRange(allPermissionsForThisRoles);
            }

            var claims = new List<Claim>
            {
                new("Id", user.Id),
                new("Email", user.Email),
                new("First name", user.FirstName),
                new("Phone number", user.PhoneNumber)
            }.Union(roleClaims);

            return claims;
        }
    }
}
