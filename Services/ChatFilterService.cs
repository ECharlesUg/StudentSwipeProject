using System.Threading.Tasks;

namespace StudentSwipe.Services
{
    public interface ChatFilterService
    {
        Task<string> FilterMessageAsync(string input);
    }
}
