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
/// This controller contains api functions for the collections data
/// it is contected to Collections table and contains get, put, post and delete calls
/// </summary>
namespace CollectionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionsController : ControllerBase
    {
        //database context
        private readonly TravelAppApiContext _context;

        public CollectionsController(TravelAppApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET : a call to get reviews
        /// </summary>
        /// <returns>list of reviews</returns>
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Collections>>> GetAllUserCollections(int userId)
        {
            var collections = await _context.Collections.Where(e => e.UserId == userId).ToListAsync();
            return collections;
        }

        /// <summary>
        /// GET : a call to get user collection by user id
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="pageNum">page number</param>
        /// <returns>a list of user collection data</returns>
        [HttpGet("userId/{id}")]
        public async Task<ActionResult<IEnumerable<Collections>>> GetUserCollections(int id, int pageNum)
        {
            //get user collection
            int pagination = pageNum * 10;
            var collections = await _context.Collections.Where(r => r.UserId == id).Skip(pagination - 10).Take(10).ToListAsync();
            //check if user collection were not found 
            if (collections == null)
            {
                return NotFound();
            }
            //return user collections
            return collections;
        }


        /// <summary>
        /// GET : a call to get the collections item by user id and collection name
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="collectionName">collection name</param>
        /// <param name="pageNum">page number</param>
        /// <returns></returns>
        [HttpGet("getCollectionItem")]
        public async Task<ActionResult<IEnumerable<Collections>>> GetUserCollectionsItem(int id, string collectionName, int pageNum)
        {
            //get user collection items 
            int pagination = pageNum * 10;
            var collections = await _context.Collections.Where(r => r.UserId == id && r.Name == collectionName).Skip(pagination - 10).Take(10).ToListAsync();
            //check if user collection were not found 
            if (collections == null)
            {
                return NotFound();
            }
            //return user collections items
            return collections;
        }

        /// <summary>
        /// POST : a call to create a new collection or add new collection item 
        /// </summary>
        /// <param name="collections">new collection data</param>
        /// <returns>new collection data</returns>
        [HttpPost]
        public async Task<ActionResult<Collections>> PostCollections(Collections collections)
        {
            //try to create a new collection and catch any error
            try
            {
                //check the new collection name and id (if collection does not exists)
                if(collections.PlaceId == 0 && collections.PlaceName == null)
                {
                    //get a list of collection ids from the database
                    var col_id = await _context.Collections.Select(m => m.Id).ToListAsync();
                    //set the maximum id and increase the id number for the new collection
                    collections.Id = col_id.Max() + 1;
                }
                //if collection exisit 
                else
                {
                    //get the collection id 
                    var s = await _context.Collections.Where(v => v.Name == collections.Name && v.UserId == collections.UserId).Select(m => m.Id).FirstAsync();
                    collections.Id = s;
                }

                //get list of rows ids
                var c = await _context.Collections.Select(m => m.CollectionId).ToListAsync();
                //set the maximum id and increase the id number for the new collection
                collections.CollectionId = c.Max() + 1;
                
                //set the dates to today's date
                collections.DateCreated = DateTime.Today;
                collections.LastUpdate = DateTime.Today;
           
                //add new collection and save the changes
                _context.Collections.Add(collections);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CollectionsExists(collections.CollectionId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            //call get method and return the new collection data
            return CreatedAtAction("GetUserCollections", new { id = collections.CollectionId }, collections);
        }

        /// <summary>
        /// DELETE : a call to delete a user collection by user id and collection name
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        [HttpDelete("deleteCollection")]
        public async Task<ActionResult<Collections>> DeleteCollections(int userId, string collectionName)
        {
            //get the collection items
            var collections = await _context.Collections.Where(d => d.UserId == userId && d.Name == collectionName).ToListAsync();
            //check if user collection items were not found 
            if (collections == null)
            {
                return NotFound();
            }
            //delete the collection items
            foreach(var collection in collections)
            {
                _context.Collections.Remove(collection);
            }
            //save the changes and return no content
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// DELETE : a call to delete a single collection item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("deleteCollectionItem/id/{id}")]
        public async Task<ActionResult<Collections>> DeleteCollectionItem(int id)
        {
            //get collection item
            var collection = await _context.Collections.Where(d => d.CollectionId == id).FirstAsync();
            //check if user collection item was not found 
            if (collection == null)
            {
                return NotFound();
            }
            //remove a collection item
            _context.Collections.Remove(collection);
            //save the changes and return no content
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// this function checks if a record exist in the database 
        /// </summary>
        /// <param name="id">city id</param>
        /// <returns>boolean value</returns>
        private bool CollectionsExists(int id)
        {
            return _context.Collections.Any(e => e.CollectionId == id);
        }
    }
}
