using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using StudentSwipe.Models;
using System.Linq;
using System.Threading.Tasks;

public class AuthenticationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthenticationController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("Register")]

    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View("~/Views/Home/Login.cshtml");
    }

    public IActionResult AboutUs()
    {
        return View("~/Views/Home/AboutUs.cshtml");
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
            await _signInManager.SignInAsync(user, isPersistent: false);
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

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        if (!result.Succeeded)
        {
            TempData["LoginError"] = "Invalid login attempt.";
            return RedirectToAction("Login");
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("AboutUs");
    }



    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok("Logout successful");
    }
}
