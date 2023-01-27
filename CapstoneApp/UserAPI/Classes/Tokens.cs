using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPI.Classes
{
    /// <summary>
    /// this class will handle creating token object for loging authrization
    /// </summary>
    public class Tokens
    {
        //token value
        public string Token { get; set; }
        // user first and last name 
        public string UserName { get; set; }
        //user id
        public int Id { get; set; }
        //user role (user or admin)
        public string UserRole { get; set; }
        //refresh token value
        public string RefreshToken { get; set; }
    }
}
