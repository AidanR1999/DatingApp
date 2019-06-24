using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// attempts to log the user in
        /// </summary>
        /// <param name="username">username user has entered</param>
        /// <param name="password">password user has entered</param>
        /// <returns>user object</returns>
        public async Task<User> Login(string username, string password)
        {
            //get user from database
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            //if username doesnt exist, return null
            if(user == null)
                return null;
            
            //if passwords don't match, return null
            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;
            
            //return user on success
            return user;
        }

        /// <summary>
        /// checks that the password hashes match
        /// </summary>
        /// <param name="password">password user has entered</param>
        /// <param name="passwordHash">hashed password of potential user</param>
        /// <param name="passwordSalt">salt key of potential user</param>
        /// <returns>boolean</returns>
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            //dispose of cryptography after use for security
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                //generate hash
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                //compare hashes using for loop due to byte arrays
                for(int i = 0; i < computedHash.Length; i++)
                {
                    //if discrepency, return false
                    if(computedHash[i] != passwordHash[i])
                        return false;
                } 
            }

            //on success, return true
            return true;
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

        /// <summary>
        /// check if user exists
        /// </summary>
        /// <param name="username">username entered</param>
        /// <returns>boolean</returns>
        public async Task<bool> UserExists(string username)
        {
            //if user exists, return true
            if(await _context.Users.AnyAsync(x => x.Username == username))
                return true;

            //on fail, return false
            return false;
        }
    }
}