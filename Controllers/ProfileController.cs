using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSwipe.Models;
using StudentSwipe.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Linq;

namespace StudentSwipe.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (profile == null) return RedirectToAction("CreateOrEdit");

            return View(profile); 
        }

        public async Task<IActionResult> CreateOrEdit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");


            ViewBag.StudyOptions = new List<string> { "Morning", "Afternoon", "Evening" };
            ViewBag.RoommateOptions = new List<string> { "Quiet", "Night Owl", "Smoker", "Non-Smoker" };

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            return View(profile ?? new Profile
            {
                UserType = EmailHelper.GetUserTypeFromEmail(user.Email)
            });
        }

       


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(Profile profileModel, IFormFile ProfilePicture, [FromForm] string[] selectedStudyPrefs, [FromForm] string[] selectedRoommatePrefs)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var existing = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

            profileModel.StudyPreferences = selectedStudyPrefs != null ? string.Join(",", selectedStudyPrefs) : "";
            profileModel.RoommatePreferences = selectedRoommatePrefs != null ? string.Join(",", selectedRoommatePrefs) : "";


            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePicture.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)); 

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePicture.CopyToAsync(stream);
                }


                profileModel.ProfilePictureUrl = "/uploads/" + fileName;
            }
            else if (existing != null)
            {

                profileModel.ProfilePictureUrl = existing.ProfilePictureUrl;
            }

            if (existing != null)
            {
                existing.FullName = profileModel.FullName;
                existing.Interests = profileModel.Interests;
                existing.Age = profileModel.Age;
                existing.StudyPreferences = profileModel.StudyPreferences;
                existing.RoommatePreferences = profileModel.RoommatePreferences;
                existing.ProfilePictureUrl = profileModel.ProfilePictureUrl;
            }
            else
            {
                profileModel.UserId = user.Id;
                profileModel.Email = user.Email;
                profileModel.UserType = EmailHelper.GetUserTypeFromEmail(user.Email);
                _context.Profiles.Add(profileModel);
            }

            await _context.SaveChangesAsync();


            return RedirectToAction("MyProfile");
        }

        [HttpGet]
        public async Task<IActionResult> AllProfiles()
        {
            var profiles = await _context.Profiles.ToListAsync();
            return View(profiles);
        }

        [HttpPost]
        public async Task<IActionResult> SendInvite(int profileId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Logic to handle invite sending, e.g., save to DB, send notification, etc.
            TempData["Message"] = "Invite sent!";
            return RedirectToAction("AllProfiles");
        }

        [HttpPost]
        public async Task<IActionResult> RejectInvite(int profileId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Logic to handle invite rejection
            TempData["Message"] = "Invite rejected.";
            return RedirectToAction("AllProfiles");
        }

    }
}
