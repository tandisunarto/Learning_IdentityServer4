using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
// using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using jwt.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
// using Microsoft.IdentityModel.Tokens;

namespace jwt.Controllers
{
    public class AuthController : Controller
    {
        private UserManager<IdentityUser> _userManager { get; set; }

        public AuthController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Basic(string ReturnUrl)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, "Tandi Sunarto"),
                new Claim(ClaimTypes.Email, "tandi.sunarto@test.com"),
                new Claim("Level", "14"),
                new Claim("role", "superman")
            };
            var licenseClaims = new List<Claim> {
                new Claim(ClaimTypes.Email, "tandi.sunarto@test.com"),
                new Claim("Type", "C")
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Basic Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "License Identity");

            var userPrincipal = new ClaimsPrincipal(new [] {claimsIdentity, licenseIdentity});

            HttpContext.SignInAsync(userPrincipal);

            return Redirect(ReturnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateTokens(LoginViewModel model)
        {
            // username: tandi@live.com password: Password!1
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                // key must be at least 16 chars for SHA256
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecretKey"));

                var token = new JwtSecurityToken(
                    issuer: "http://halo.world.com",
                    audience: "http://halo.world.com",
                    expires: DateTime.UtcNow.AddHours(1),
                    claims: claims,
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            return Unauthorized();
        }
    }
}