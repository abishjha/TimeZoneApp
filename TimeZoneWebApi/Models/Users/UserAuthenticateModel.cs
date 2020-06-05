using System.ComponentModel.DataAnnotations;

namespace TimeZoneWebApi.Models.Users
{
    public class UserAuthenticateModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}