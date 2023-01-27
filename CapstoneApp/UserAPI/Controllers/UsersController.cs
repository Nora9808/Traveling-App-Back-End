using System;
using System.Collections.Generic;
using TravelAppUtility.DTO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAppUtility.Data;
using TravelAppUtility.Models;

/// <summary>
/// Nora Mamo
/// 27-10-2022
/// 
/// This controller user api functions for the user data
/// it is contected to user table and contains get, put, post calls
/// the controller authrize when login or signup
/// </summary>
namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        //database context
        private readonly TravelAppApiContext _context;
        //jwt for token and authorization
        private readonly IJWTManagerRepository _jWTManager;

        public UsersController(TravelAppApiContext context, IJWTManagerRepository jWTManager)
        {
            this._jWTManager = jWTManager;
            _context = context;
        }

        /// <summary>
        /// GET : a call to get all users
        /// </summary>
        /// <returns>users data list</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }


        /// <summary>
        /// GET : a call to get user by user id
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>user data</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUsers(int id)
        {
            //find the user
            var users = await _context.Users.FindAsync(id);
            //return null if user does not exist
            if (users == null)
            {
                return NotFound();
            }
            //return user data
            return users;
        }

       /// <summary>
       /// PUT : a call to update users data
       /// </summary>
       /// <param name="id">user id</param>
       /// <param name="users">user data</param>
       /// <returns>no content</returns>
        [HttpPut("{id}")]
        [ActionName(nameof(PutUsers))]
        public async Task<IActionResult> PutUsers(int id, Users users)
        {
            //return bad request if id is not valid
            if (id != users.UserId)
            {
                return BadRequest();
            }

            //try to update the user data and catch any error
            try
            {
                //if user password is empty
                if (users.Password == null || users.Password == "")
                {
                    //find the user password in the database
                    var password = await _context.Users.Where(a => a.UserId == id).Select(x => x.Password).FirstAsync();
                    //set the user password to their exisiting password in the database
                    users.Password = password;
                }
                //if password is not empty
                else
                {
                    //encrypt password and save it to the user data
                    users.Password = BCrypt.Net.BCrypt.HashPassword(users.Password);
                }

                //get the date created from database
                var date = await _context.Users.Where(a => a.UserId == id).Select(x => x.DateCreated).FirstAsync();
                //set the user datecreated to the same value from the database
                users.DateCreated = date;
                //update latest update to today's date
                users.LastUpdate = DateTime.Today;

                //enter and save the changes
                _context.Entry(users).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(users.FirstName, users.LastName, users.Email))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }


        /// <summary>
        /// POST : a call to authenticate user logings
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate(Users users)
        {
            //a variable that will save the token return from
            //authenticate method
            var token = _jWTManager.Authenticate(users, _context);

            //if token is null, user not authorized
            if (token == null)
            {
                return Unauthorized();
            }

            //retrun the token value
            return Ok(token);
        }


       /// <summary>
       /// POST : a call to create a new user (when the user sign up)
       /// </summary>
       /// <param name="users">user data</param>
       /// <returns>new user data</returns>
        [HttpPost]
        [ActionName(nameof(PostUsers))]
        public async Task<ActionResult<Users>> PostUsers(Users users)
        {
            //try creating new user and catch any error   
            try
            {
                //get list of users ids
                var user_id = await _context.Users.Select(m => m.UserId).ToListAsync();
                //set the maximum id and increase the id number for the new user
                users.UserId = user_id.Max() + 1;
                
                //set date created to today's date and latest update to null
                users.DateCreated = DateTime.Today;
                users.LastUpdate = null;

                //encrypt the password
                users.Password = BCrypt.Net.BCrypt.HashPassword(users.Password);

                // add user to users table and save changes
                _context.Users.Add(users);
                await _context.SaveChangesAsync();

                //call get method to return new user data
                return CreatedAtAction("GetUsers", new { id = users.UserId });
            }
            catch (DbUpdateException)
            {
                // return conflict if user exists
                if (UsersExists(users.FirstName, users.LastName, users.Email))
                {
                    return Conflict(new { message = $"An existing record with the id '{users.UserId}' was already found." });
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// this function checks if a record exist in the database 
        /// </summary>
        /// <param name="fName">user firat name</param>
        /// <param name="lName">user last name</param>
        /// <param name="Email">user email</param>
        /// <returns>boolea value</returns>
        private bool UsersExists(String fName, String lName, String Email)
        {
            return _context.Users.Any(e => e.FirstName == fName && e.LastName == lName || e.Email == Email);
        }
    }
}
