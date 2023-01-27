using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAppUtility.Data;
using TravelAppUtility.Models;


/// <summary>
/// Nora Mamo
/// 26-10-2022
/// 
/// This controller contains api functions for the user events list page
/// it is contected to userEvents table and contains get ,post and delete calls
/// </summary>
namespace CalenderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserEventsController : ControllerBase
    {
        //database context
        private readonly TravelAppApiContext _context;

        public UserEventsController(TravelAppApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET : a call to get all the user event
        /// </summary>
        /// <returns>events list</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserEvents>>> GetUserEvents()
        {
            return await _context.UserEvents.ToListAsync();
        }

        /// <summary>
        /// POST : a call to create a new user event 
        /// </summary>
        /// <param name="userEvents">user event array</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<UserEvents>> PostUserEvents(UserEvents userEvents)
        {
            //try to create a new user event and catch any error
            try
            {
                //check for empty user id and event id
                if(userEvents.eventId <= 0 || userEvents.UserId == null || userEvents.UserId <= 0)
                {
                    return BadRequest();
                }
                //get a list of events id from the database
                var u = await _context.UserEvents.Select(m => m.Id).ToListAsync();
                //set the new record id to the maximun id in the database increased by 1
                userEvents.Id = u.Max() + 1;
                //set the dates to today's date
                userEvents.DateCreated = DateTime.Today;
                userEvents.LastUpdate = DateTime.Today;
                //add and save the event
                _context.UserEvents.Add(userEvents);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserEventsExists(userEvents.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            //return to get method to return the event details
            return CreatedAtAction("GetUserEvents", new { id = userEvents.Id }, userEvents);
        }

        /// <summary>
        /// DELETE: a call to delete a user event by user event id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserEvents>> DeleteUserEvents(int id)
        {
            //get the user events list with same event id
            var userEvents = await _context.UserEvents.Where(e => e.eventId == id).ToListAsync();
            //return not found if the user event does not exist in the table
            if (userEvents == null)
            {
                return NotFound();
            }

            //remove the user events with the same event id
            foreach (var userEvent in userEvents)
            {
                _context.UserEvents.Remove(userEvent);
            }

            //save changes and return no content
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// this function checks if a record exist in the database 
        /// </summary>
        /// <param name="id">user event id</param>
        /// <returns>bolean value</returns>
        private bool UserEventsExists(int id)
        {
            return _context.UserEvents.Any(e => e.Id == id);
        }
    }
}
