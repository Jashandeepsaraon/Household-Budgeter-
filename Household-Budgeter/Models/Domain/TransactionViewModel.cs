﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models.Domain
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerId { get; set; }

    }
}