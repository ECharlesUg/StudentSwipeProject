using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Mvc.RazorPages; // Add this namespace
using StudentSwipe.Models; 


public class ProfileCreationRoomateModel : PageModel
{
    [BindProperty]
    public Profile Profile { get; set; }

    public void OnGet()
    {
        Profile = new Profile(); 
    }
}