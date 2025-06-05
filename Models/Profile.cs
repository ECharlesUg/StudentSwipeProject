using System.ComponentModel.DataAnnotations;

namespace StudentSwipe.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Email { get; set; }

        public int Age { get; set; }

        public int Grade { get; set; }
        public string? UniversityYear { get; set; }
        public string? HSCourses  { get; set; }
        public string? UniCourses { get; set; }

        [Display(Name = "Study Preferences")]
        public string StudyPreferences { get; set; } 

        [Display(Name = "Roommate Preferences")]
        public string? RoommatePreferences { get; set; }  

        public string ProfilePictureUrl { get; set; }

        public string Interests { get; set; }

        public string UserType { get; set; }
        public string? RoommateType { get; set; } // WithHousing or WithoutHousing
        public string? HousingDescription { get; set; }
        public decimal? MonthlyRent { get; set; }
        public string? RentSplitPlan { get; set; }
        public string? Expectations { get; set; }
    }
}