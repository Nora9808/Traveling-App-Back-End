using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Spire.Xls;
using TravelAppUtility.Data;
using TravelAppUtility.Models;

/// <summary>
/// Nora Mamo
/// 27-10-2022
/// 
/// This controller continent api functions for the continent data
/// it is contected to Continent table and contains get, put, post and delete calls
/// </summary>
namespace ContinentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContinentsController : ControllerBase
    {
        //database context
        private readonly TravelAppApiContext _context;

        public ContinentsController(TravelAppApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET : a call to get all countries
        /// </summary>
        /// <returns>list of countries</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Continent>>> GetReviews()
        {
            return await _context.Continent.ToListAsync();
        }

        /// <summary>
        /// GET : a call to get all the cities
        /// </summary>
        /// <returns>cities list</returns>
        [HttpGet("paginatedContinent")]
        public String GetAllContinents(int pageNum)
        {
            int pagination = pageNum * 10;

            //initialize a work book object
            Workbook wb = new Workbook();
            //load the file
            wb.LoadFromFile(@"C:\Excel_File\Travel_App_Data.xlsx");
            //get the city sheet
            Worksheet sheet = wb.Worksheets[1];
            DataTable dt = sheet.ExportDataTable(sheet.Range, false);

            List<object> dataList = new List<object>();

            int counter = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (counter != 0)
                {
                    dataList.Add(row.ItemArray);
                }
                counter++;
            }

            List<object> paginatedDataList = dataList.Skip(pagination - 10).Take(10).ToList();

            string output = JsonConvert.SerializeObject(paginatedDataList);

            return output;
        }

        /// <summary>
        /// GET : a call to get a continent by continent id
        /// </summary>
        /// <param name="id">continent id</param>
        /// <returns>a continent information array</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Continent>> GetContinent(int id)
        {
            //get the continent 
            var continent = await _context.Continent.FindAsync(id);
            //return not found if id does not exist
            if (continent == null)
            {
                return NotFound();
            }
            //return continent details array
            return continent;
        }

        /// <summary>
        /// GET : a call to get a continent by continent name
        /// </summary>
        /// <param name="continentName">continent name</param>
        /// <returns>continent data</returns>
        [HttpGet("continentName/{continentName}")]
        public async Task<ActionResult<Continent>> GetContinentName(String continentName)
        {
            //get continent by continent name
            var continent = await _context.Continent.FirstOrDefaultAsync(x => x.ContinentName.ToLower() == continentName.ToLower());
            //return not found if id does not exist
            if (continent == null)
            {
                return NotFound();
            }
            //return continent details array
            return continent;
        }

        /// <summary>
        /// PUT : a call to update a continent information
        /// </summary>
        /// <param name="id">continent id</param>
        /// <param name="continent">continent data</param>
        /// <returns>no content</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContinent(int id, Continent continent)
        {
            //return bad request if id does not exist
            if (id != continent.ContinentId)
            {
                return BadRequest();
            }

            //try to update the continent and catch any error
            try
            {
                //initialize a work book object
                Workbook wb = new Workbook();
                //load the file
                wb.LoadFromFile(@"C:\Excel_File\Travel_App_Data.xlsx");
                //get the continent sheet
                Worksheet sheet = wb.Worksheets[1];
                //first row record in the sheet
                int rowNum = 2;
                //boolean to track the updated row
                bool isUpdate = false;

                //while the row is not updated
                while (!isUpdate)
                {
                    //get the id cell range 
                    string range = "A" + rowNum;
                    //get the cell value
                    int cell_value = Int32.Parse(sheet.Range[range.ToString()].Value);
                    //check if id from sheet is equal to parameter id
                    if (cell_value == id)
                    {
                        //get the continent name and discription cell range
                        string nameRange = "B" + rowNum;
                        string descRange = "C" + rowNum;
                        sheet.Range[nameRange.ToString()].Value = continent.ContinentName;
                        sheet.Range[descRange.ToString()].Value = continent.Description;
                        //set boolean to true and break out of loop
                        isUpdate = true;
                        break;
                    }
                    else
                    {
                        //if row not found, increase row number and set boolean to false
                        rowNum++;
                        isUpdate = false;
                    }
                }
                //save the changes
                wb.SaveToFile(@"C:\Excel_File\Travel_App_Data.xlsx", FileFormat.Version2013);
                //update the last update date to today's date
                continent.LastUpdate = DateTime.Today;
                //update and save the changes in the database context
                _context.Entry(continent).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContinentExists(id))
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
        /// POST : a call to enter a new continent to Continent table
        /// </summary>
        /// <param name="continent">new continent array</param>
        /// <returns>new continent data</returns>
        [HttpPost]
        public async Task<ActionResult<Continent>> PostContinent(Continent continent)
        {
            //try to create new continent and catch any error
            try
            {
                //get a list of continen ids
                var con_id = await _context.Continent.Select(m => m.ContinentId).ToListAsync();
                //set the maximum id and increase the id number for the new continent
                continent.ContinentId = con_id.Max() + 1;

                //initialize a work book object
                Workbook wb = new Workbook();
                //load the file
                wb.LoadFromFile(@"C:\Excel_File\Travel_App_Data.xlsx");
                //get the continent sheet
                Worksheet sheet = wb.Worksheets[1];

                //get the last row number increased by 1 
                int num = sheet.LastDataRow + 1;
                //set the ranges for continent name, description and id cells
                string nameRange = "B" + num;
                string descRange = "C" + num;
                string idRange = "A" + num;
                //set the new values in the cells
                sheet.Range[idRange.ToString()].Value = (continent.ContinentId).ToString();
                sheet.Range[nameRange.ToString()].Value = continent.ContinentName;
                sheet.Range[descRange.ToString()].Value = continent.Description;
                //save the changes
                wb.SaveToFile(@"C:\Excel_File\Travel_App_Data.xlsx", FileFormat.Version2013);
                
                //set the date created to today's date and last updated to null
                continent.DateCreated = DateTime.Today;
                continent.LastUpdate = null;
                
                //add and save the new city
                _context.Continent.Add(continent);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ContinentExists(continent.ContinentId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            //call get method to return the new continent array
            return CreatedAtAction("GetContinent", new { id = continent.ContinentId }, continent);
        }

        /// <summary>
        /// DELETE : a call to delelte a continent by continent id
        /// </summary>
        /// <param name="id">continent id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Continent>> DeleteContinent(int id)
        {
            //find the continent in database context
            var continent = await _context.Continent.FindAsync(id);
            //return not found if id does not exist
            if (continent == null)
            {
                return NotFound();
            }

            //initialize a work book object
            Workbook wb = new Workbook();
            //load the file
            wb.LoadFromFile(@"C:\Excel_File\Travel_App_Data.xlsx");
            //get the continent sheet
            Worksheet sheet = wb.Worksheets[1];
            //first row record in the sheet
            int rowNum = 2;
            //boolean to track the deleted row
            bool isDelete = false;

            //while the row is not deleted
            while (!isDelete)
            {
                //set the id cell range
                string range = "A" + rowNum;
                //get the cell value
                int cell_value = Int32.Parse(sheet.Range[range.ToString()].Value);

                //check if id from sheet is equal to parameter id
                if (cell_value == id)
                {
                    //delete the row
                    sheet.DeleteRow(rowNum);
                    //set the boolean to true and break out of loop
                    isDelete = true;
                    break;
                }
                else
                {
                    //if row not deleted, increase row number and set boolean to false
                    rowNum++;
                    isDelete = false;
                }
            }
            //save the changes in the file sheet
            wb.SaveToFile(@"C:\Excel_File\Travel_App_Data.xlsx", FileFormat.Version2013);
            //remove and save changes in the database context
            _context.Continent.Remove(continent);
            await _context.SaveChangesAsync();

            return continent;
        }

        /// <summary>
        /// this function checks if a record exist in the database 
        /// </summary>
        /// <param name="id">continent id</param>
        /// <returns>boolean value</returns>
        private bool ContinentExists(int id)
        {
            return _context.Continent.Any(e => e.ContinentId == id);
        }
    }
}
