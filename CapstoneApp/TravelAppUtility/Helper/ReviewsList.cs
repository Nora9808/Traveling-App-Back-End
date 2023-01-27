using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TravelAppUtility.Helper
{
    /// <summary>
    /// this class will handle creating a reviews list 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReviewsList<T>
    {
       //review date created
        public DateTime DateCreated { get; set; }
        //number of reviews
        public int NumberOfReviews { get; set; }

        public List<T> Items { get; set; }

        /// <summary>
        /// revview constructor to set the date and number values
        /// </summary>
        /// <param name="dateCreated">review date created</param>
        /// <param name="numOfReviews">number of reviews</param>
        public ReviewsList(DateTime dateCreated, int numOfReviews)
        {
            DateCreated = dateCreated;
            NumberOfReviews = numOfReviews;
        }

      

       
    }
}
