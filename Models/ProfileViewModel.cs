namespace StudentSwipe.Models
{
    public class ProfileViewModel
    {
        public string FullName { get; set; }
        public string Interests { get; set; }
        public string ProfilePictureUrl { get; set; }

        public string StudyPreferences { get; set; }
        public string? RoommatePreferences { get; set; }
        public string UserType { get; set; }  
    }
}
