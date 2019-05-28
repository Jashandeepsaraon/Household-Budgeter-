using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class CategoriesViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string HouseholdName { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Owner { get; set; }
        public string OwnerId { get; set; }
    }
}