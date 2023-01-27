using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using TravelAppUtility.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TravelAppUtility.Data;
using TravelAppUtility.Models;
using UserAPI.Classes;

namespace UserAPI
{
    internal class JWTManagerRepository : IJWTManagerRepository
    {
        private readonly IConfiguration iconfiguration;
        public JWTManagerRepository(IConfiguration iconfiguration)
        {
            this.iconfiguration = iconfiguration;
        }

        /// <summary>
        /// this method will authorize user logins
        /// </summary>
        /// <param name="users">user data</param>
        /// <param name="context">database context</param>
        /// <returns>token object</returns>
        public Tokens Authenticate(Users users, TravelAppApiContext context)
        {
            //get user by email
            var user = context.Users.SingleOrDefault(user => user.Email == users.Email);

            //check if user null or password inncorrect
            if (user == null || !BCrypt.Net.BCrypt.Verify(users.Password, user.Password))
            {
                return null;
            }

            //generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(iconfiguration["JWT:Key"]);
            //clain identity by first and last name
            var claimIdentity = new ClaimsIdentity();
            claimIdentity.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
            claimIdentity.AddClaim(new Claim(ClaimTypes.Name, user.LastName));
            //create token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimIdentity,
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //set the user role
            var userRole = "";
            if(user.IsAdmin == true)
            {
                userRole = "Admin";
            }
            else
            {
                userRole = "User";
            }

            //return token, username, id and user role
            return new Tokens { Token = tokenHandler.WriteToken(token), UserName = user.FirstName + " " + user.LastName, UserRole = userRole, Id = user.UserId };
        }
    }
}