using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSwipe.Models;
using StudentSwipe.Helpers;
namespace StudentSwipe.Controllers
{


    [Authorize]
    public class RoomateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RoomateController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult RoomateMatch()
        {
            return View("Views/RoomMatch/RoomateMatch.cshtml"); 
        }
        public IActionResult RoomateRole()
        {
            return View("Views/RoomMatch/RoomateRole.cshtml");
        }
        public IActionResult RoomateProfile()
        {
            return View("Views/RoomMatch/ProfileCreationRoomate.cshtml");
        }
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "WithHousing", "SeekingHousing" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> SelectRole()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            return View(); // This view will show options: WithHousing / SeekingHousing
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectRole(string selectedRole)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (selectedRole == "WithHousing" || selectedRole == "SeekingHousing")
            {
                await _userManager.AddToRoleAsync(user, selectedRole);
                return RedirectToAction("CreateOrEdit");
            }

            ModelState.AddModelError("", "Invalid role selection.");
            return View();
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
                existing.RoommatePreferences = profileModel.RoommatePreferences;
                existing.ProfilePictureUrl = profileModel.ProfilePictureUrl;
                existing.MonthlyRent = profileModel.MonthlyRent;
                existing.RentSplitPlan = profileModel.RentSplitPlan;
                existing.Expectations = profileModel.Expectations;
                existing.HousingDescription = profileModel.HousingDescription;
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
            var user = await _userManager.GetUserAsync(User);
            var profiles = await _context.Profiles
                .Where(p => p.UserId != user.Id)
                .ToListAsync();

            return View(profiles);
        }
        [HttpPost]
        public async Task<IActionResult> SendInvite(int profileId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Get the target profile
            var targetProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId);
            if (targetProfile == null) return NotFound();

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.LikerId == user.Id && l.LikedId == targetProfile.UserId);

            if (existingLike == null)
            {
                _context.Likes.Add(new Like
                {
                    LikerId = user.Id,
                    LikedId = targetProfile.UserId, // ✅ now matching UserId correctly
                    IsLiked = true
                });
                await _context.SaveChangesAsync();
            }

            TempData["Message"] = "Invite sent!";
            return RedirectToAction("AllProfiles");
        }



        [HttpPost]
        public async Task<IActionResult> RejectInvite(int profileId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Get the profile you're rejecting
            var targetProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId);
            if (targetProfile == null) return NotFound();

            var existing = await _context.Likes
                .FirstOrDefaultAsync(l => l.LikerId == user.Id && l.LikedId == targetProfile.UserId);

            if (existing == null)
            {
                _context.Likes.Add(new Like
                {
                    LikerId = user.Id,
                    LikedId = targetProfile.UserId,
                    IsLiked = false
                });
                await _context.SaveChangesAsync();
            }

            TempData["Message"] = "Invite rejected.";
            return RedirectToAction("AllProfiles");
        }



        [HttpGet]
        public async Task<IActionResult> MyLikes()
        {
            var user = await _userManager.GetUserAsync(User);
            var likedProfileIds = await _context.Likes
                .Where(l => l.LikerId == user.Id && l.IsLiked)
                .Select(l => l.LikedId)
                .ToListAsync();

            var likedProfiles = await _context.Profiles
                .Where(p => likedProfileIds.Contains(p.UserId))
                .ToListAsync();

            return View(likedProfiles);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Authentication");

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (profile != null) _context.Profiles.Remove(profile);

            var likes = _context.Likes.Where(l => l.LikerId == user.Id || l.LikedId == user.Id);
            _context.Likes.RemoveRange(likes);

            await _context.SaveChangesAsync();

            await _userManager.DeleteAsync(user);
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Authentication");
        }




    }
}
