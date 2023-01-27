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
/// This controller country api functions for the country data
/// it is contected to country table and contains get, put, post and delete calls
/// </summary>
namespace CountryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        //database context
        private readonly TravelAppApiContext _context;

        public CountriesController(TravelAppApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET : a call to get all countries
        /// </summary>
        /// <returns>list of countries</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Country>>> GetReviews()
        {
            return await _context.Country.ToListAsync();
        }


        /// <summary>
        /// GET : a call to get all the cities
        /// </summary>
        /// <returns>cities list</returns>
        [HttpGet("paginatedCountry")]
        public String GetAllCountries(int pageNum)
        {
            int pagination = pageNum * 10;

            //initialize a work book object
            Workbook wb = new Workbook();
            //load the file
            wb.LoadFromFile(@"C:\Excel_File\Travel_App_Data.xlsx");
            //get the city sheet
            Worksheet sheet = wb.Worksheets[2];
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
        /// GET : a call to get a country by country id
        /// </summary>
        /// <param name="id">country id</param>
        /// <returns>a country information array</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(int id)
        {
            //get the country
            var country = await _context.Country.FindAsync(id);
            //return not found if id does not exist
            if (country == null)
            {
                return NotFound();
            }
            //return country details array
            return country;
        }


        /// <summary>
        /// GET : a call to get a country by country name
        /// </summary>
        /// <param name="countryName">country name</param>
        /// <returns>country data</returns>
        [HttpGet("countryName/{countryName}")]
        public async Task<ActionResult<Country>> GetCountryName(String countryName)
        {
            //get country by country name
            var country = await _context.Country.FirstOrDefaultAsync(x => x.CountryName.ToLower() == countryName.ToLower());
            //return not found if id does not exist
            if (country == null)
            {
                return NotFound();
            }
            //return country details array
            return country;
        }


        /// <summary>
        /// PUT : a call to update a country information
        /// </summary>
        /// <param name="id">country id</param>
        /// <param name="country">country data</param>
        /// <returns>no content</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, Country country)
        {
            //return bad request if id does not exists
            if (id != country.CountryId)
            {
                return BadRequest();
            }

            //try to update the country and catch any error
            try
            {
                //initialize a work book object
                Workbook wb = new Workbook();
                //load the file
                wb.LoadFromFile(@"C:\Excel_File\Travel_App_Data.xlsx");
                //get the country sheet
                Worksheet sheet = wb.Worksheets[2];
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
                        sheet.Range[nameRange.ToString()].Value = country.CountryName;
                        sheet.Range[descRange.ToString()].Value = country.Description;
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
                country.LastUpdate = DateTime.Today;
                //update and save the changes in the database context
                _context.Entry(country).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
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
        /// POST : a call to enter a new country to Country table
        /// </summary>
        /// <param name="country">new country array</param>
        /// <returns>new country data</returns>
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(Country country)
        {
            //try to create new country and catch any error
            try
            {
                //get a list of country ids
                var con_id = await _context.Country.Select(m => m.CountryId).ToListAsync();
                //set the maximum id and increase the id number for the new country
                country.CountryId = con_id.Max() + 1;

                //initialize a work book object
                Workbook wb = new Workbook();
                //load the file
                wb.LoadFromFile(@"C:\Excel_File\Travel_App_Data.xlsx");
                //get the country sheet
                Worksheet sheet = wb.Worksheets[2];

                //get the last row number increased by 1 
                int num = sheet.LastDataRow + 1;
                //set the ranges for continent name, description and id cells
                string nameRange = "B" + num;
                string descRange = "C" + num;
                string idRange = "A" + num;
                //set the new values in the cells
                sheet.Range[idRange.ToString()].Value = (country.CountryId).ToString();
                sheet.Range[nameRange.ToString()].Value = country.CountryName;
                sheet.Range[descRange.ToString()].Value = country.Description;
                //save the changes
                wb.SaveToFile(@"C:\Excel_File\Travel_App_Data.xlsx", FileFormat.Version2013);
                
                //set the date created to today's date and last updated to null
                country.DateCreated = DateTime.Today;
                country.LastUpdate = null;
                
                //add and save the new country
                _context.Country.Add(country);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CountryExists(country.CountryId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            //call get method to return the new country array
            return CreatedAtAction("GetCountry", new { id = country.CountryId }, country);
        }

        /// <summary>
        /// DELETE : a call to delelte a country by country id
        /// </summary>
        /// <param name="id">country id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Country>> DeleteCountry(int id)
        {
            //find the country n the database context
            var country = await _context.Country.FindAsync(id);
            //return not found if id does not exist
            if (country == null)
            {
                return NotFound();
            }

            //initialize a work book object
            Workbook wb = new Workbook();
            //load the file
            wb.LoadFromFile(@"C:\Excel_File\Travel_App_Data.xlsx");
            //get the country sheet
            Worksheet sheet = wb.Worksheets[2];
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
            _context.Country.Remove(country);
            await _context.SaveChangesAsync();

            return country;
        }

        /// <summary>
        /// this function checks if a record exist in the database 
        /// </summary>
        /// <param name="id">country id</param>
        /// <returns>boolean value</returns>
        private bool CountryExists(int id)
        {
            return _context.Country.Any(e => e.CountryId == id);
        }
    }
}
