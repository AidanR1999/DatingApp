namespace DatingApp.API.Models
{
    /// <summary>
    /// User in the system
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}