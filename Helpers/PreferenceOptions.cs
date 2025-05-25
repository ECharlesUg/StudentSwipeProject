using System.Collections.Generic;

namespace StudentSwipe.Helpers
{
    public static class PreferenceOptions
    {
        public static readonly List<string> StudyOptions = new() { "Quiet", "Group", "Online", "Late Night", "In-Person" };
        public static readonly List<string> RoommateOptions = new() { "Quiet", "Social", "Non-Smoker", "Early Riser", "Clean" };
    }
}