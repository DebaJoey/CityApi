using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        /// <summary>
        /// Requets body for authentication
        /// </summary>
        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        /// <summary>
        /// Represents a user in the city information system.
        /// </summary>
        private class CityInfoUser
        {
            public int UserId { get; set; }

            public string UserName { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string City { get; set; }


            /// <summary>
            /// Initializes a new instance of the <see cref="CityInfoUser"/> class.
            /// </summary>
            /// <param name="userId">The unique identifier of the user.</param>
            /// <param name="userName">The username of the user.</param>
            /// <param name="firstName">The first name of the user.</param>
            /// <param name="lastName">The last name of the user.</param>
            /// <param name="city">The city where the user resides.</param>
            public CityInfoUser(
                int userId,
                string userName,
                string firstName,
                string lastName,
                string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Authenticates a user based on the provided credentials and generates a JWT token upon successful authentication.
        /// </summary>
        /// <param name="authenticationRequestBody">The authentication request body containing the username and password.</param>
        /// <returns>An HTTP response containing a JWT token upon successful authentication.</returns>
        [HttpPost("authenticate")]

        public ActionResult<string> Authenticate(
            AuthenticationRequestBody authenticationRequestBody)
        {
            //Step 1 Validate user email and password
            var user = ValidateUserCredentials(
                authenticationRequestBody.UserName,
                authenticationRequestBody.Password);

            if(user == null)
            {
                return Unauthorized();
            }

            //Step 2 Create a token

            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));

            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            // the claims that
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("given_name", user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));


            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);

        }

        private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        {

            //This is for demonstartion purposes so this dummy user is returned
            return new CityInfoUser(
                1,
                userName ?? "",
                "Kevin",
                "Dockx",
                "Antwerp");
        }
    }
}

