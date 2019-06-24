using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    /// <summary>
    /// Serializes the data recieved from registering
    /// </summary>
    public class UserForRegisterDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "You must specify password between 4 and 20 characters")]
        public string Password { get; set; }
    }
}