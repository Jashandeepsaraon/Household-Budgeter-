using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class CategoriesBindingModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string HouseholdName { get; set; }
        [Required]
        public string Description { get; set; }
    }
}