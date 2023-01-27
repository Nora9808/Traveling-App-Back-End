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
/// This controller contains api functions for the events list page
/// it is contected to Events table and contains get, put, post and delete calls
/// </summary>
namespace CalenderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        //database context
        private readonly TravelAppApiContext _context;

        public EventsController(TravelAppApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET : a call to get all the event
        /// </summary>
        /// <returns>events list</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Events>>> GetEvents()
        {
            var events = await _context.Events.ToListAsync();
            return events;
        }

        /// <summary>
        /// GET : a call to get all the event
        /// </summary>
        /// <param name="pageNum">page number</param>
        /// <returns>events list</returns>
        [HttpGet("PageNum/{pageNum}")]
        public async Task<ActionResult<IEnumerable<Events>>> GetPaginatedEvents(int pageNum)
        {
            int pagination = pageNum * 10;
            var events = await _context.Events.Skip(pagination - 10).Take(10).ToListAsync();
            return events;
        }

        /// <summary>
        /// GET: a call to get the events by start date
        /// </summary>
        /// <param name="startDate">event start data</param>
        /// <returns>events list</returns>
        [HttpGet("startDate/{startDate}")]
        public async Task<ActionResult<Events>> GetEventsByDate(DateTime startDate)
        {
            //get the events by the start date
            var events = await _context.Events.Where(e => e.StartDate == startDate).FirstAsync();

            //check if events not found
            if (events == null)
            {
                return NotFound();
            }

            //return events list
            return events;
        }

        /// <summary>
        /// PUT: a call to update an event 
        /// </summary>
        /// <param name="id">event id</param>
        /// <param name="events">event details array</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvents(int id, Events events)
        {
            //return bad request if event was not found
            if (id != events.EventsId)
            {
                return BadRequest();
            }

            //try to update the event and catch any error
            try
            {   
                //get the date created for the event to be updated
                var date = await _context.Events.Where(a => a.EventsId == id).Select(x => x.DateCreated).FirstAsync();
                //set the date created to the date in the database
                events.DateCreated = date;
                //set last updated time to today's date
                events.LastUpdate = DateTime.Today;
                //enter and save the changes in the database
                _context.Entry(events).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventsExists(id))
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
        /// POST : a call to create a new event 
        /// </summary>
        /// <param name="events">event details array</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Events>> PostEvents(Events events)
        {
            //try to create a new event and catch any error
            try
            {
                //get a list of the ids in the event table
                var u = await _context.Events.Select(m => m.EventsId).ToListAsync();
                //set the maximum id and increase the id number for the new event
                events.EventsId = u.Max() + 1;
                //set the dates to today's date
                events.DateCreated = DateTime.Today;
                events.LastUpdate = DateTime.Today;
                //add and save the new user event
                _context.Events.Add(events);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EventsExists(events.EventsId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            //return to get method to return the event details
            return CreatedAtAction("GetEvents", new { id = events.EventsId }, events);
        }

        /// <summary>
        /// DELETE: a call to delete a event by event id
        /// </summary>
        /// <param name="id">user event id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Events>> DeleteEvents(int id)
        {
            //get the event
            var events = await _context.Events.FindAsync(id);
            //return not found if the event does not exist in the table
            if (events == null)
            {
                return NotFound();
            }
            //delete the event from the database and save changes
            _context.Events.Remove(events);
            await _context.SaveChangesAsync();
            return events;
        }

        /// <summary>
        /// this function checks if a record exist in the database 
        /// </summary>
        /// <param name="id">event id</param>
        /// <returns>boolean value</returns>
        private bool EventsExists(int id)
        {
            return _context.Events.Any(e => e.EventsId == id);
        }
    }
}
