using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DishesMenu.Models
{
    // this is how we can extend default AspNetUsers table with new colums
    public class ApplicationUser : IdentityUser
    {
        // we will add the properties that we want to append to to the IdentityUser table

        public string Name { get; set; }

        public string StreetAddress { get; set; }

        // public string PhoneNumber { get; set; } are not added beacause this column is already exist in db

        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }
}
