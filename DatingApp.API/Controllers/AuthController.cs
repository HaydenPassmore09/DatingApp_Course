using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using DatingApp.API.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
            _config = config;
        }

        /*The attribute [HttpPost("register")] specifies the specific url path 
        * allong with the route attribute at the top of the class definition.
        * for example the following action will be called when the url is queried
        * '<domain>/api/Auth/register' 
        */
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if (await _repo.UserExists(userForRegisterDto.Username))
            {
                return BadRequest("Username already exists");
            }

            var userToCreate = new User
            {
                UserName = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201); //TODO - change this to created at route
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            //First check if we have a user and their username and 
            //password matches what is stored in the database for that particular user
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            //Start Building up the JSON Web token

            //Our token will contain 2 claims one is going to be the users ID and the other is going to be the users username
            var claims = new[]{
                    new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                    new Claim(ClaimTypes.Name, userFromRepo.UserName)
                };

            //In order to make sure that this toke is a valid token the server needs to sign the token

            //Create a security key 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            //use the above key as part of the signing credentials and encrypting this key with a hashing algorithim 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Following this we start creating the token

            //Create a token discripter passing our clams as the subject
            //setting the expiry date
            // then passing the signing credentials
            var tokenDiscriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            //Create the JwtSecurity token handler which allows us 
            //to create the token based on the token discriptor
            var tokenHandler = new JwtSecurityTokenHandler();

            //Create the token
            var token = tokenHandler.CreateToken(tokenDiscriptor);

            var user = _mapper.Map<UserForListDto>(userFromRepo);

            //Write the token into the response that we send back to the client
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }
    }
}