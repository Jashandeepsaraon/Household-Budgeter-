using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models.Domain
{
    public class HouseholdsBindingModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public virtual List<ApplicationUser> Users { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public string OwnerId { get; set; }
    }
}