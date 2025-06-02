using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StudentSwipe.Helpers;
using StudentSwipe.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container  
builder.Services.AddControllersWithViews();

// Add database context  
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity  
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Bind EmailSettings from appsettings.json
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register SmtpEmailSender with injected settings
builder.Services.AddTransient<IEmailSender>(sp =>
{
    var emailSettings = sp.GetRequiredService<IOptions<EmailSettings>>().Value;
    return new SmtpEmailSender(
        emailSettings.Host,
        emailSettings.Port,
        emailSettings.Username,
        emailSettings.Password,
        emailSettings.FromEmail
    );
});


builder.Services.AddSignalR();

var app = builder.Build();


app.MapHub<ChatHub>("/chathub");


// Configure middleware  
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();



    if (!context.SchoolDomains.Any())
    {
        var domains = SchoolDomainSeeder.GetPredefinedDomains();
        context.SchoolDomains.AddRange(domains);
        context.SaveChanges();
    }
}

app.Run();
