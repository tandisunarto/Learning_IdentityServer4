using System;
using System.IdentityModel.Tokens.Jwt;
// using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using jwt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Core;

namespace jwt.Controllers
{
    public class AuthController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private IEmailService _emailService;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailService emailService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // username: tandi@live.com password: Password!1
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))            
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (signInResult.Succeeded) {
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

                    return View("Tokens", (
                        new JwtSecurityTokenHandler().WriteToken(token),
                        token.ValidTo
                    ));
                }

                if (! await _userManager.IsEmailConfirmedAsync(user))
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var link = Url.Action(nameof(VerifyEmail), "Auth", new { userId = user.Id, code = code }); //, Request.Scheme, Request.Host);
                    await _emailService.SendAsync("tandi@local.info", "Email Verification", $"<a href=\"{link}\">Verify Email</a>");

                    return RedirectToAction("EmailVerification");
                };
                return View();
            }

            return Unauthorized();
        }

        public IActionResult EmailVerification ()
        {
            return View();
        }

        public async Task<IActionResult> VerifyEmail (string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null ) return BadRequest("Unable to verify your email !");

            var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, code);
            if (confirmEmailResult.Succeeded)
                return View();
            
            return BadRequest("Your email verification failed!");
        }
    }
}