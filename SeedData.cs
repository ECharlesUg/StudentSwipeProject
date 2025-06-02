public static class SchoolDomainSeeder
{
    public static List<SchoolDomain> GetPredefinedDomains()
    {
        return new List<SchoolDomain>
        {
            new SchoolDomain { Domain = ".uwinnipeg.ca", SchoolName = "University of Winnipeg", SchoolType = "University" },
            new SchoolDomain { Domain = "myumanitoba.ca", SchoolName = "University of Manitoba", SchoolType = "University" },
            new SchoolDomain { Domain = "live.wsd1.org", SchoolName = "Winnipeg School Division", SchoolType = "HighSchool" },
            new SchoolDomain { Domain = "lrsd.net", SchoolName = "Louis Riel School Division", SchoolType = "HighSchool" },
            new SchoolDomain { Domain = "smail.pembinatrails.ca", SchoolName = "Pembina Trails School Division", SchoolType = "HighSchool" },
            new SchoolDomain { Domain = "7oaks.org", SchoolName = "Seven Oaks School Division", SchoolType = "HighSchool" },
            new SchoolDomain { Domain = "sjsd.net", SchoolName = "St. James-Assiniboia School Division", SchoolType = "HighSchool" },
            new SchoolDomain { Domain = "retsd.mb.ca", SchoolName = "River East Transcona School Division", SchoolType = "HighSchool" },
            new SchoolDomain { Domain = "student.mitt.ca", SchoolName = "Manitoba Institute of Trades and Technology", SchoolType = "University" },
            new SchoolDomain { Domain = "academic.rrc.ca", SchoolName = "Red River College of Applied Arts, Science & Technology", SchoolType = "University" },
            new SchoolDomain { Domain = "assiniboine.net.", SchoolName = "Assiniboine Community College", SchoolType = "University" },
            new SchoolDomain { Domain = "learning.icmanitoba.ca", SchoolName = "International College of Manitoba", SchoolType = "University" },
            new SchoolDomain { Domain = "cmu.ca", SchoolName = "Canadian Mennonite University", SchoolType = "University" },
        };
    }
}
