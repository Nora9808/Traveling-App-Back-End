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
/// 27-10-2022
/// 
/// This controller favorites api functions for the favorites data
/// it is contected to favorites table and contains get, put, post and delete calls
/// </summary>
namespace FavoriteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        //database context
        private readonly TravelAppApiContext _context;

        public FavoritesController(TravelAppApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: a call to get the user favorites data by user id
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="pageNum">page number</param>
        /// <returns></returns>
        [HttpGet("userId/{id}")]
        public async Task<ActionResult<IEnumerable<Favorites>>> GetUserFavorites(int id, int pageNum)
        {
            //get user's favorties
            int pagination = pageNum * 10;
            var favorites = await _context.Favorites.Where(r => r.UserId == id).AsNoTracking().Skip(pagination - 10).Take(10).ToListAsync();
            //return not found if user's favorites do not exist
            if (favorites == null)
            {
                return NotFound();
            }
            return favorites;
        }

       /// <summary>
       /// POST : a call to create a new user favorites (add place to favorites list)
       /// </summary>
       /// <param name="favorites">new favorites data</param>
       /// <returns>no content</returns>
        [HttpPost]
        public async Task<ActionResult<Favorites>> PostFavorites(Favorites favorites)
        {
            //try to create a favorites and catch any error
            try
            {
                //get favorites ids from database
                var f = await _context.Favorites.Select(m => m.FavoritesId).ToListAsync();
                //set the maximum id and increase the id number for the new favorites
                favorites.FavoritesId = f.Max() + 1;
                //det the dates to today's date
                favorites.DateCreated = DateTime.Today;
                favorites.LastUpdate = DateTime.Today;

                //add new favorites and save the changes
                _context.Favorites.Add(favorites);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FavoritesExists(favorites.FavoritesId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        /// <summary>
        /// DELETE : a call to delete user favorties by user id
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Favorites>> DeleteFavorites(int id)
        {
            //get user favorites
            var favorites = await _context.Favorites.FindAsync(id);
            //return not found of user favorites does not exist in database
            if (favorites == null)
            {
                return NotFound();
            }
            //remove and save the changes
            _context.Favorites.Remove(favorites);
            await _context.SaveChangesAsync();
            return favorites;
        }

        /// <summary>
        /// this function checks if a record exist in the database 
        /// </summary>
        /// <param name="id">favorites id</param>
        /// <returns>boolean value</returns>
        private bool FavoritesExists(int id)
        {
            return _context.Favorites.Any(e => e.FavoritesId == id);
        }
    }
}
