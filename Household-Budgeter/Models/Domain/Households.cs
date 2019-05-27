using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models.Domain
{
    public class Households
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public virtual List<ApplicationUser> Users { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public string OwnerId { get; set; }
        public virtual List<ApplicationUser> InviteUsers { get; set; }
        public virtual List<Categories> Categories { get; set; }
        public virtual List<BankAccount> BankAccounts { get; set; }

        public Households()
        {
            DateCreated = DateTime.Now;
        }  
    }
}