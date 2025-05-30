using Microsoft.AspNetCore.Mvc; // Add this namespace
using Microsoft.AspNetCore.Mvc.RazorPages; // Add this namespace
using StudentSwipe.Models; // Add the namespace where the 'Profile' class is defined


// Pages/RoomMatch/ProfileCreationRoomate.cshtml.cs
public class ProfileCreationRoomateModel : PageModel
{
    [BindProperty]
    public Profile Profile { get; set; }

    public void OnGet()
    {
        Profile = new Profile(); // or load from database if editing
    }
}