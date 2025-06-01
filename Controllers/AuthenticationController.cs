using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using StudentSwipe.Models;
using System.Linq;
using System.Threading.Tasks;

public class AuthenticationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;

    public AuthenticationController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View("~/Views/Home/Login.cshtml");
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Home/Login.cshtml", model);

        var emailDomain = model.Email.Split('@').Last().ToLower();

        var isAllowed = _context.SchoolDomains
            .Any(d => emailDomain.EndsWith(d.Domain.ToLower()));

        if (!isAllowed)
        {
            ModelState.AddModelError("Email", "Please use a valid school email address.");
            return View("~/Views/Home/Login.cshtml", model);
        }

        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Authentication",
                new { userId = user.Id, token = token }, Request.Scheme);

            var message = $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>";
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email", message);

            TempData["RegisterMessage"] = "Account created. Please check your email to confirm your account.";
            return RedirectToAction("Login");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View("~/Views/Home/Login.cshtml", model);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromForm] LoginModel model, string returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            TempData["LoginError"] = "Invalid login attempt.";
            return RedirectToAction("Login");
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            TempData["LoginError"] = "Invalid login attempt.";
            return RedirectToAction("Login");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordValid)
        {
            TempData["LoginError"] = "Invalid login attempt.";
            return RedirectToAction("Login");
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            TempData["UnconfirmedEmail"] = user.Email;
            return RedirectToAction("EmailNotConfirmed");
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        TempData["LoginSuccess"] = "Login successful."; // <-- Added line

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("MyProfile", "Profile");
    }

    [HttpGet("EmailNotConfirmed")]
    public IActionResult EmailNotConfirmed()
    {
        var email = TempData["UnconfirmedEmail"]?.ToString();
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login");

        return View("EmailNotConfirmed", model: email);
    }

    [HttpPost("ResendVerificationEmail")]
    public async Task<IActionResult> ResendVerificationEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || await _userManager.IsEmailConfirmedAsync(user))
            return RedirectToAction("Login");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = Url.Action("ConfirmEmail", "Authentication",
            new { userId = user.Id, token = token }, Request.Scheme);

        var message = $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>";
        await _emailSender.SendEmailAsync(user.Email, "Confirm your email", message);

        TempData["RegisterMessage"] = "Verification email sent again.";
        return RedirectToAction("Login");
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok("Logout successful");
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null)
            return RedirectToAction("Login");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return RedirectToAction("Login");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            TempData["RegisterMessage"] = "Email confirmed successfully. You can now login.";
            return RedirectToAction("Login");
        }

        TempData["RegisterMessage"] = "Error confirming email.";
        return RedirectToAction("Login");
    }
}
