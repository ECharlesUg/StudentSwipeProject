using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentSwipe.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser> 
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SchoolDomain> SchoolDomains { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Like> Likes { get; set; }

}
