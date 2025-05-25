namespace StudentSwipe.Helpers
{
    public static class EmailHelper
    {
        public static string GetUserTypeFromEmail(string email)
        {
            var highSchoolDomains = new List<string> { "@live.wsd1.org", "@lrsd.net", "@smail.pembinatrails.ca", "@7oaks.org", "@retsd.mb.ca" };
            var universityDomains = new List<string> { "@.uwinnipeg.ca", "@umanitoba.ca", "@student.mitt.ca", "@academic.rrc.ca", "@assiniboine.net.", "@learning.icmanitoba.ca", "@cmu.ca" };

            email = email.ToLower();

            if (universityDomains.Any(d => email.EndsWith(d))) return "University";
            if (highSchoolDomains.Any(d => email.EndsWith(d))) return "HighSchool";

            return "Unknown";
        }
    }


}