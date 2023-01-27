using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAppUtility.Data;
using TravelAppUtility.Models;
using Spire.Xls;
using System.Data;
using Newtonsoft.Json;

/// <summary>
/// Nora Mamo
/// 26-10-2022
/// 
/// This controller contains api functions for the city data
/// it is contected to City table and contains get, put, post and delete calls
/// </summary>
namespace CityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        //database context
        private readonly TravelAppApiContext _context;

        public CitiesController(TravelAppApiContext context)
        {
            _context = context;
        }

        
        /// <summary>
        /// GET : a call to get all paginated cities
        /// </summary>
        /// <returns>cities list</returns>
        [HttpGet("paginatedCiy")]
        public String GetAllCities(int pageNum)
        {
            int pagination = pageNum * 10;

            //initialize a work book object
            Workbook wb = new Workbook();
            //load the file
            wb.LoadFromFile(@"C:\Excel_File\Travel_App_Data.xlsx");
            //get the city sheet
            Worksheet sheet = wb.Worksheets[3];
            DataTable dt = sheet.ExportDataTable(sheet.Range, false);

            List<object> dataList = new List<object>();

            int counter = 0;
            foreach (DataRow row in dt.Rows)
            {
                if(counter != 0)
                {
                    dataList.Add(row.ItemArray);
                }
                counter++;
            }

            List<object> paginatedDataList =  dataList.Skip(pagination-10).Take(10).ToList();

            string output = JsonConvert.SerializeObject(paginatedDataList);

            return output;
        }

        /// <summary>
        /// GET : a call to get a city by city id
        /// </summary>
        /// <param name="id">city id</param>
        /// <returns>a city information array</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            //find the city from database context
            var city = await _context.City.FindAsync(id);
            //return not found if id does not exist
            if (city == null)
            {
                return NotFound();
            }
            //return city details array
            return city;
        }

        /// <summary>
        /// GET : a call to get a cit by city name
        /// </summary>
        /// <param name="cityName">city name</param>
        /// <returns>city data</returns>
        [HttpGet("cityName/{cityName}")]
        public async Task<ActionResult<City>> GetCityName(String cityName)
        {
            //get the city by city name
            var city = await _context.City.FirstOrDefaultAsync(x => x.CityName.ToLower() == cityName.ToLower());
            //return not found if id does not exist
            if (city == null)
            {
                return NotFound();
            }
            //return city details array
            return city;
        }

        /// <summary>
        /// PUT : a call to update a city information
        /// </summary>
        /// <param name="id">city id</param>
        /// <param name="city">city data</param>
        /// <returns>no content</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, City city)
        {
            //return bad request if id does not exist
            if (id != city.CityId)
            {
                return BadRequest();
            }

            //try to update the city and catch any error
            try
            {
                //initialize a work book object
                Workbook wb = new Workbook();
                //load the file
                wb.LoadFromFile(@"C:\Excel_File\Travel_App_Data.xlsx");
                //get the city sheet
                Worksheet sheet = wb.Worksheets[3];
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
                        //get the city name and discription cell range
                        string nameRange = "B" + rowNum;
                        string descRange = "C" + rowNum;
                        //update the cells with the new data
                        sheet.Range[nameRange.ToString()].Value = city.CityName;
                        sheet.Range[descRange.ToString()].Value = city.Description;
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
                city.LastUpdate = DateTime.Now;
          
                //update and save the changes in the database context
                _context.Entry(city).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetCity", new { id = city.CityId }, city);
        }

        /// <summary>
        /// POST : a call to enter a new city to City table
        /// </summary>
        /// <param name="city">new city array</param>
        /// <returns>new city data</returns>
        [HttpPost]
        public async Task<ActionResult<City>> PostCity(City city)
        {
            //try to create a new city and catch any error
            try
            {
                //get a list of cities ids
                var u = await _context.City.Select(m => m.CityId).ToListAsync();
                //set the maximum id and increase the id number for the new city
                city.CityId = u.Max() + 1;

                //initialize a work book object
                Workbook wb = new Workbook();
                //load the file
                wb.LoadFromFile(@"C:\Excel_File\Travel_App_Data.xlsx");
                //get the city sheet
                Worksheet sheet = wb.Worksheets[3];

                //get the last row number increased by 1 
                int num = sheet.LastDataRow+ 1;
                //set the ranges for city name, description and id cells
                string nameRange = "B" + num;
                string descRange = "C" + num;
                string idRange = "A" + num;
                //set the new values in the cells
                sheet.Range[idRange.ToString()].Value = (city.CityId).ToString();
                sheet.Range[nameRange.ToString()].Value = city.CityName;
                sheet.Range[descRange.ToString()].Value = city.Description;
                //save the changes
                wb.SaveToFile(@"C:\Excel_File\Travel_App_Data.xlsx", FileFormat.Version2013);
                //set the date created to today's date and last updated to null
                city.DateCreated = DateTime.Today;
                city.LastUpdate = null;
                //add and save the new city
                _context.City.Add(city);
               await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CityExists(city.CityId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            //call get method to return the new city array
            return CreatedAtAction("GetCity", new { id = city.CityId }, city);
        }

        /// <summary>
        /// DELETE : a call to delelte a city by city id
        /// </summary>
        /// <param name="id">city id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<City>> DeleteCity(int id)
        {
            //find the city in database context
            var city = await _context.City.FindAsync(id);
            //return not found if id does not exist
            if (city == null)
            {
                return NotFound();
            }


            //initialize a work book object
            Workbook wb = new Workbook();
            //load the file
            wb.LoadFromFile(@"C:\Excel_File\Travel_App_Data.xlsx");
            //get the city sheet
            Worksheet sheet = wb.Worksheets[3];
            //set the first data row in the excel file
            int rowNum = 2;
            //boolean to track the deleted data
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
            _context.City.Remove(city);
            await _context.SaveChangesAsync();
            return city;
        }

        /// <summary>
        /// this function checks if a record exist in the database 
        /// </summary>
        /// <param name="id">city id</param>
        /// <returns>boolean value</returns>
        private bool CityExists(int id)
        {
            return _context.City.Any(e => e.CityId == id);
        }
    }
}
