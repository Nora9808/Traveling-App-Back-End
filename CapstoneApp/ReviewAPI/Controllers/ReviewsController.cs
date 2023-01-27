 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAppUtility.Data;
using TravelAppUtility.Helper;
using TravelAppUtility.Models;

/// <summary>
/// Nora Mamo
/// 27-10-2022
/// 
/// This controller reviews api functions for the reviews data
/// it is contected to reviews table and contains get, put, post and delete calls
/// </summary>
namespace ReviewAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly TravelAppApiContext _context;

        public ReviewsController(TravelAppApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET : a call to get reviews
        /// </summary>
        /// <returns>list of reviews</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reviews>>> GetReviews()
        {
            return await _context.Reviews.ToListAsync();
        }

        /// <summary>
        /// GET : a call to get all the reviews by review id
        /// </summary>
        /// <param name="id">review id</param>
        /// <returns>place reviews list</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Reviews>> GetReviews(int id)
        {
            //get the review data
            var reviews = await _context.Reviews.FindAsync(id);
            //return not found if id does not exist
            if (reviews == null)
            {
                return NotFound();
            }
            //return review data
            return reviews;
        }

        /// <summary>
        /// GET : a call to get all the reviews by user id
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="pageNum">page number</param>
        /// <returns>user reviews list</returns>
        [HttpGet("userId/{id}")]
        public async Task<ActionResult<IEnumerable<Reviews>>> GetUserReviews(int id, int pageNum)
        {
            //get a list of user's reviews
            int pagination = pageNum * 10;
            var reviews = await _context.Reviews.Where(r => r.UserId == id).Skip(pagination - 10).Take(10).ToListAsync();

            //return not found if id does not
            if (reviews == null)
            {
                return NotFound();
            }

            //int pageNum = num * 10;
            //var pagenated_reviews = reviews.Skip(pageNum - 10).Take(10).ToList();

            //return user reviews list
            return reviews;
        }

        /// <summary>
        /// GET : a call to get all the reviews for a specific place by place id
        /// </summary>
        /// <param name="id">place id</param>
        /// <param name="pageNum">page number</param>
        /// <returns>reviews list</returns>
        [HttpGet("placeId/{id}")]
        public async Task<ActionResult<IEnumerable<Reviews>>> GetPlaceReviews(int id, int pageNum)
        {
            //get the place review
            int pagination = pageNum * 10;
            var reviews = await _context.Reviews.Where(r => r.placeId == id).AsNoTracking().Skip(pagination - 10).Take(10).ToListAsync();
            //return not found if the id does not exist
            if (reviews == null)
            {
                return NotFound();
            }
            //return list of place reviews
            return reviews;
        }

        /// <summary>
        /// PUT : a call to update a review data by review id
        /// </summary>
        /// <param name="id">review id</param>
        /// <param name="reviews">review data</param>
        /// <returns>updated review data</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReviews(int id, Reviews reviews)
        {
            //return bad request if review id is not valid
            if (id != reviews.RevewsId)
            {
                return BadRequest();
            }

            //try to update review data and catch any error
            try
            {
                //update latest date to todays date
                reviews.LastUpdate = DateTime.Today;

                //update and save changes
                _context.Entry(reviews).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //call get method to get the updated review data
            return CreatedAtAction("GetReviews", new { id = reviews.RevewsId }, reviews);
        }

        /// <summary>
        /// POST : a call to create a new user review
        /// </summary>
        /// <param name="reviews">new review data</param>
        /// <returns>new review data</returns>
        [HttpPost]
        public async Task<ActionResult<Reviews>> PostReviews(Reviews reviews)
        {
            //try to create a new review and catch any error
            try
            {
                //get kist of review ids
                var rev_id = await _context.Reviews.Select(m => m.RevewsId).ToListAsync();
                //set the maximum id and increase the id number for the new review
                reviews.RevewsId = rev_id.Max() + 1;
                //set the date created to today's date and lastest update to null
                reviews.DateCreated = DateTime.Today;
                reviews.LastUpdate = null;

                //add the new review and save the changes
                _context.Reviews.Add(reviews);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ReviewsExists(reviews.RevewsId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            //call get method to return the new review data
            return CreatedAtAction("GetReviews", new { id = reviews.RevewsId }, reviews);
        }

        /// <summary>
        /// DELETE: a call to delete a user review by review id
        /// </summary>
        /// <param name="id">review id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Reviews>> DeleteReviews(int id)
        {
            //get the review data
            var reviews = await _context.Reviews.FindAsync(id);
            //return not found if id does not exist
            if (reviews == null)
            {
                return NotFound();
            }
            //remove and save the changes
            _context.Reviews.Remove(reviews);
            await _context.SaveChangesAsync();

            return reviews;
        }

        /// <summary>
        /// this function checks if a record exist in the database 
        /// </summary>
        /// <param name="id">review id</param>
        /// <returns>boolean value</returns>
        private bool ReviewsExists(int id)
        {
            return _context.Reviews.Any(e => e.RevewsId == id);
        }
    }
}
