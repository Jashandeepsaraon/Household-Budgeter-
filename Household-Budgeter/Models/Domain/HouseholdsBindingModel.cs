using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models.Domain
{
    public class HouseholdsBindingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual List<ApplicationUser> Users { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public string OwnerId { get; set; }
    }
}