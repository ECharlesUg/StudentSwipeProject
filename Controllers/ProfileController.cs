using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSwipe.Models;
using StudentSwipe.Helpers;

namespace StudentSwipe.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Role Selection & Routing
        [HttpGet]
        public async Task<IActionResult> SelectRole()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            return View();
        }

        // ProfileController.cs
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectRole(string selectedRole)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Add user to role (optional, if using ASP.NET Identity roles)
            if (selectedRole == "WithHousing" || selectedRole == "SeekingHousing")
            {
                await _userManager.AddToRoleAsync(user, selectedRole);

                // Update the user's profile UserType
                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
                if (profile != null)
                {
                    profile.UserType = selectedRole;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // If profile doesn't exist, create one
                    _context.Profiles.Add(new Profile
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        UserType = selectedRole
                    });
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("RoomateProfile", new { userType = selectedRole });
            }

            ModelState.AddModelError("", "Invalid role selection.");
            return View();
        }

        // StudyProfile
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
                existing.Grade = profileModel.Grade;
                existing.HSCourses = profileModel.HSCourses;
                existing.UniCourses = profileModel.UniCourses;
                existing.UniversityYear = profileModel.UniversityYear;
                existing.StudyPreferences = profileModel.StudyPreferences;
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

        /*//Roomate
        public IActionResult RoommateMatch() => View("Views/RoomMatch/RoomateMatch.cshtml");
        public IActionResult RoommateRole() => View("Views/RoomMatch/RoomateRole.cshtml");

        public async Task<IActionResult> RoomateProfile(string userType)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            ViewBag.RoommateOptions = new List<string> { "Quiet", "Night Owl", "Smoker", "Non-Smoker" };
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (profile == null)
            {
                profile = new Profile
                {
                    UserId = user.Id,
                    Email = user.Email,
                    UserType = userType 
                };
            }
            else if (!string.IsNullOrEmpty(userType))
            {
                profile.UserType = userType;
                await _context.SaveChangesAsync();
            }

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoomateProfile(Profile profileModel, IFormFile ProfilePicture, [FromForm] string[] selectedStudyPrefs, [FromForm] string[] selectedRoommatePrefs)
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
                profileModel.UserType = "SeekingHousing";
                _context.Profiles.Add(profileModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("MyProfile");
        }*/


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

            var targetProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId);
            if (targetProfile == null) return NotFound();

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.LikerId == user.Id && l.LikedId == targetProfile.UserId);

            if (existingLike == null)
            {
                _context.Likes.Add(new Like
                {
                    LikerId = user.Id,
                    LikedId = targetProfile.UserId,
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

        [HttpGet]
        public async Task<IActionResult> PendingInvites()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var likers = await _context.Likes
                .Where(l => l.LikedId == user.Id && l.IsLiked)
                .Select(l => l.LikerId)
                .ToListAsync();

            var profiles = await _context.Profiles
                .Where(p => likers.Contains(p.UserId))
                .ToListAsync();

            return View(profiles);
        }

        [HttpGet]
        public async Task<IActionResult> Matches()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var iLiked = await _context.Likes
                .Where(l => l.LikerId == user.Id && l.IsLiked)
                .Select(l => l.LikedId)
                .ToListAsync();

            var mutualLikes = await _context.Likes
                .Where(l => iLiked.Contains(l.LikerId) && l.LikedId == user.Id && l.IsLiked)
                .Select(l => l.LikerId)
                .ToListAsync();

            var matchedProfiles = await _context.Profiles
                .Where(p => mutualLikes.Contains(p.UserId))
                .ToListAsync();

            return View(matchedProfiles);
        }


        public async Task<bool> IsMutualMatchAllowed(string userId1, string userId2)
        {
            var user1 = await _context.Users.FindAsync(userId1);
            var user2 = await _context.Users.FindAsync(userId2);

            var profile1 = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId1);
            var profile2 = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId2);

            bool isMutualLike = await _context.Likes.AnyAsync(l =>
                l.LikerId == userId1 && l.LikedId == userId2 && l.IsLiked) &&
                await _context.Likes.AnyAsync(l =>
                l.LikerId == userId2 && l.LikedId == userId1 && l.IsLiked);

            return isMutualLike && profile1.UserType == profile2.UserType;
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

        // Optional: For initializing roles
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
    }
}
