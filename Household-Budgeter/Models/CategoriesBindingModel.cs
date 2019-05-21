using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class CategoriesBindingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HouseholdName { get; set; }
        public string Description { get; set; }
    }
}