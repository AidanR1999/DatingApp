using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    /// <summary>
    /// serializes data received from user login
    /// </summary>
    public class UserForLoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}