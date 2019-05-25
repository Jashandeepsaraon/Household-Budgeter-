using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models.Domain
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DateUpdated { get; set; }
        public decimal Amount { get; set; }
        public bool Void { get; set; }
        public int CategoriesId { get; set; }
        public virtual Categories Categories { get; set; }

        public int BankAccountId { get; set; }
        public virtual BankAccount BankAccount { get; set; }

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public Transaction()
        {
            DateCreated = DateTime.Now;
        }
    }
}