using System;
using System.Collections.Generic;
using System.Linq;
using TravelAppUtility.DTO;
using System.Threading.Tasks;
using TravelAppUtility.Data;
using TravelAppUtility.Models;
using UserAPI.Classes;

namespace UserAPI
{
    public interface IJWTManagerRepository
    {
        Tokens Authenticate(Users users, TravelAppApiContext context);
    }
}