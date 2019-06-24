using System;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.API.Controllers
{
    /// <summary>
    /// Handles the data when authorising users
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository authRepo, IConfiguration config)
        {
            _authRepo = authRepo;
            _config = config;
        }

        /// <summary>
        /// registers users and sends request
        /// </summary>
        /// <param name="userForRegisterDTO">user registration data</param>
        /// <returns>request </returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegisterDTO)
        {
            //normalise username
            userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower();

            //check if username is already taken
            if(await _authRepo.UserExists(userForRegisterDTO.Username))
                return BadRequest("Username already exists");

            //create user
            var userToCreate = new User
            {
                Username = userForRegisterDTO.Username
            };

            //add user to database
            var createdUser = await _authRepo.Register(userToCreate, userForRegisterDTO.Password);

            return StatusCode(201);
        }

        /// <summary>
        /// logs the user in and generates a token
        /// </summary>
        /// <param name="userForLoginDTO">user login data</param>
        /// <returns>request with token</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO)
        {
            //login the user
            var userFromAuthRepo = await _authRepo.Login(userForLoginDTO.Username.ToLower(), userForLoginDTO.Password);

            //check that user is logged in
            if(userFromAuthRepo == null)
                return Unauthorized();

            //create claims using user id and username
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromAuthRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromAuthRepo.Username)
            };

            //generate key from secret token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //generate and hash credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //create description of token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires =  DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            
            //instantiate a token handler to create token
            var tokenHandler = new JwtSecurityTokenHandler();

            //create token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //return request, passing through newly written token using handler
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}