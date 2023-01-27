using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TravelAppUtility.DTO
{
    /// <summary>
    /// this class will handle creating folloing user object
    /// when generating a user following list
    /// </summary>
    public class FollowingUsers
    {
        //following user id
        [Required]
        public int userId { get; set; }
        //following first name
        [Required]
        public string FirstName { get; set; }
        //following last name
        [Required]
        public string LastName { get; set; }
    }
}
