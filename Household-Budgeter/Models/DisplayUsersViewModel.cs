using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class DisplayUsersViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public List<string> Users { get; set; }
    }
}