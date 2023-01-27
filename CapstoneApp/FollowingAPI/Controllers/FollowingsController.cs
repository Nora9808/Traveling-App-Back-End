using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAppUtility.Data;
using TravelAppUtility.Models;
using TravelAppUtility.DTO;

/// <summary>
/// Nora Mamo
/// 27-10-2022
/// 
/// This controller following api functions for the following data
/// it is contected to following table and contains get, put, post and delete calls
/// </summary>
namespace FollowingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowingsController : ControllerBase
    {
        //database context
        private readonly TravelAppApiContext _context;

        public FollowingsController(TravelAppApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET : a call to ge all the user followings by the logged user id
        /// </summary>
        /// <param name="userId">following id</param>
        /// <returns></returns>
        [HttpGet("/api/AllFollowings/userId/{userId}")]
        public async Task<ActionResult<IEnumerable<FollowingUsers>>> GetAllFollowing(int userId)
        {
            var followings = await _context.Following.Where(f => f.UserId == userId).ToListAsync();

            //a list to store the followings data
            List<FollowingUsers> users = new List<FollowingUsers>();

            //loop through the following result
            foreach (var following in followings)
            {
                //get the user information for each following from Users table
                var user = await _context.Users.Where(f => f.UserId == following.FollowingId).Select(f => new { f.UserId, f.FirstName, f.LastName }).FirstAsync();

                //create a following object
                FollowingUsers fUser = new FollowingUsers()
                {
                    userId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };
                //add the following object to users list
                users.Add(fUser);
            }

            //return not found if the followings do not exist
            if (followings == null)
            {
                return NotFound();
            }
            //return users list
            return users;
        }


        /// <summary>
        /// GET : a call to ge the paginated list of user followings by the logged user id
        /// </summary>
        /// <param name="userId">following id</param>
        /// <param name="pageNum">page number</param>
        /// <returns></returns>
        [HttpGet("/api/Followings/userId/{userId}")]
        public async Task<ActionResult<IEnumerable<FollowingUsers>>> GetFollowing(int userId, int pageNum)
        {
            //get the followings by user id
            int pagination = pageNum * 10;
            var followings = await _context.Following.Where(f => f.UserId == userId).Skip(pagination - 10).Take(10).ToListAsync();

            //a list to store the followings data
            List<FollowingUsers> users = new List<FollowingUsers>();
            
            //loop through the following result
            foreach (var following in followings){
                //get the user information for each following from Users table
                var user = await _context.Users.Where(f => f.UserId == following.FollowingId).Select(f => new {f.UserId, f.FirstName, f.LastName }).FirstAsync();

                //create a following object
                FollowingUsers fUser = new FollowingUsers() {
                    userId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };
                //add the following object to users list
                users.Add(fUser);
            }

            //return not found if the followings do not exist
            if (followings == null)
            {
                return NotFound();
            }
            //return users list
            return users;
        }

        /// <summary>
        /// POST : a call to create a new following (follow another user)
        /// </summary>
        /// <param name="following">following data</param>
        /// <returns></returns>
        [HttpPost("/api/Followings/Follow")]
        public async Task<ActionResult<Following>> PostFollowing(Following following)
        {
           //try to create a new following and catch any error
            try
            {
                //get a list of all following ids
                var foll_id = await _context.Following.Select(m => m.Id).ToListAsync();
                //set the maximum id and increase the id number for the new following
                following.Id = foll_id.Max() + 1;
                //set the dates to today's date
                following.DateCreated = DateTime.Today;
                following.LastUpdate = DateTime.Today;
                
                //add and save the changes
                _context.Following.Add(following);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FollowingExists(following.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            //call get method to return the new following data
            return NoContent();
        }

        /// <summary>
        /// DELETE : a call to delete a following (unfollow the user)
        /// </summary>
        /// <param name="userId">logged user id</param>
        /// <param name="followingId">following user id</param>
        /// <returns></returns>
        [HttpDelete("/api/Followings/Unfollow")]
        public async Task<ActionResult<Following>> DeleteFollowing(int userId, int followingId)
        {
            //get the following data
            var following = await _context.Following.Where(f => f.UserId == userId && f.FollowingId == followingId).FirstAsync();
            //return not found if following does not exist
            if (following == null)
            {
                return NotFound();
            }
            //remove and save the changes
            _context.Following.Remove(following);
            await _context.SaveChangesAsync();
            return following;
        }

        /// <summary>
        /// this function checks if a record exist in the database 
        /// </summary>
        /// <param name="id">following id</param>
        /// <returns>boolean value</returns>
        private bool FollowingExists(int id)
        {
            return _context.Following.Any(e => e.Id == id);
        }
    }
}
