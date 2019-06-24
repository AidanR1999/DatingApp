using System;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    /// <summary>
    /// Repository for Authorisation of users
    /// </summary>
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        } 
        public Task<User> Login(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Registers the user
        /// </summary>
        /// <param name="user">User object</param>
        /// <param name="password">password entered by user</param>
        /// <returns>user object</returns>
        public async Task<User> Register(User user, string password)
        {
            //generate the hashed and salted passwords
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            //set the hashed and salted passwords to the user object
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            //add user to database
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// generates hashed and salted passwords
        /// </summary>
        /// <param name="password">password user has entered</param>
        /// <param name="passwordHash">password to be hashed</param>
        /// <param name="passwordSalt">password to be salted</param>
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //dispose of cryptography after use for security
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                //store salt key
                passwordSalt = hmac.Key;

                //generate hash
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public Task<bool> UserExists(string username)
        {
            throw new System.NotImplementedException();
        }
    }
}