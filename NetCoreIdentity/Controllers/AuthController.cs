
using System.Threading.Tasks;
using jwt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

public class AuthController : Controller
{
  public UserManager<IdentityUser> _userManager { get; }
  public SignInManager<IdentityUser> _signInManager { get; }
  public IEmailService _emailService { get; }

  public AuthController(
      UserManager<IdentityUser> userManager,
      SignInManager<IdentityUser> signInManager,
      IEmailService emailService)
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
                return RedirectToAction("Index", "Home");
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