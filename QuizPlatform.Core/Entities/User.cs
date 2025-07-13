using Microsoft.AspNetCore.Identity;
namespace QuizPlatform.Core.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
    }
}
