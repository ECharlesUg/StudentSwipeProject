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

        [Display(Name = "Study Preferences")]
        public string StudyPreferences { get; set; } 

        [Display(Name = "Roommate Preferences")]
        public string? RoommatePreferences { get; set; }  

        public string ProfilePictureUrl { get; set; }

        public string Interests { get; set; }

        public string UserType { get; set; }  



         
    }
}