using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class TransactionBindingModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public bool Void { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public int HouseholdsId { get; set; }
        public int CategoriesId { get; set; }
        public int AccountId { get; set; }
    }
}