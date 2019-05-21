﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class CategoriesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HouseholdName { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string Owner { get; set; }
        public string OwnerId { get; set; }
    }
}