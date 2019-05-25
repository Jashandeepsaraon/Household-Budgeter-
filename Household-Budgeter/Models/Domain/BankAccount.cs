﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models.Domain
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal Balance { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int HouseholdsId { get; set; }
        public virtual Households Households { get; set; }
        public virtual List<Transaction> Transactions { get; set; }

        public BankAccount()
        {
            DateCreated = DateTime.Now;
        }
    }
}