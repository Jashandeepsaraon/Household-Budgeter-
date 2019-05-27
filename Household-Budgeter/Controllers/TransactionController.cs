using Household_Budgeter.Models;
using Household_Budgeter.Models.Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Household_Budgeter.Controllers
{
    [Authorize]
    public class TransactionController : ApiController
    {
        private ApplicationDbContext DbContext;
        public TransactionController()
        {
            DbContext = new ApplicationDbContext();
        }

        public IHttpActionResult Get(int? id)
        {
            var transaction = DbContext.Transactions.FirstOrDefault(user => user.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(new TransactionViewModel
            {
                Title = transaction.Title,
                Id = transaction.Id,
                Description = transaction.Description,
                DateCreated = transaction.DateCreated,
                DateUpdated = transaction.DateUpdated,
                Amount = transaction.Amount,
                Date = transaction.Date,
                OwnerEmail = transaction.CreatedBy.Email
            });
        }

        public IHttpActionResult Get()
        {
            var model = DbContext
               .Transactions
               .Select(p => new TransactionViewModel
               {
                   Title = p.Title,
                   Id = p.Id,
                   Description = p.Description,
                   DateCreated = p.DateCreated,
                   DateUpdated = p.DateUpdated,
                   Amount = p.Amount,
                   Date = p.Date,
                   OwnerEmail = p.CreatedBy.Email
               }).ToList();
            return Ok(model);
        }

        public IHttpActionResult Post(TransactionBindingModel formdata)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var houseHold = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == formdata.HouseholdsId);
            var categories = DbContext.Categories.FirstOrDefault(p => p.Id == formdata.CategoriesId);
            var account = DbContext.BankAccounts.FirstOrDefault(p => p.Id == formdata.AccountId);
            if (houseHold == null && categories == null)
            {
                return BadRequest("Either the Household or Category is not found.");
            }
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            if (user != null && account != null && houseHold.OwnerId == userId || categories.Households.Users.Contains(user))
            {
                Transaction transaction = new Transaction();
                var userName = User.Identity.Name;
                transaction.Title = formdata.Title;
                transaction.Description = formdata.Description;
                transaction.Amount = formdata.Amount;
                account.Balance += transaction.Amount;
                transaction.Date = formdata.Date;
                transaction.BankAccountId = account.Id;
                transaction.CategoriesId = categories.Id;
                transaction.CreatedById = userId;
                DbContext.Transactions.Add(transaction);              
                //DbContext.Transactions.Add(categories);
                DbContext.SaveChanges();

                var model = new TransactionViewModel
                {
                    Id = transaction.Id,
                    Title = transaction.Title,
                    Description = transaction.Description,
                    Amount = transaction.Amount,
                    OwnerEmail = transaction.Categories.Households.Owner.Email,
                    Date = transaction.Date,
                    OwnerId = transaction.CreatedById,
                    DateCreated = transaction.DateCreated,
                    DateUpdated = transaction.DateUpdated
                };
                return Ok(model);
            }
            else
            {
                return BadRequest("You are not the owner or member of this HouseHold.");
            }
        }

        public IHttpActionResult Put(int? id, TransactionBindingModel formdata)
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var household = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == id);
            var account = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);
            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id);

            if (household == null && account == null)
            {
                return BadRequest("Either the Household or Account is not found.");
            }

            if (user != null && transaction != null && transaction.Categories.Households.OwnerId == userId  || transaction.CreatedById == userId)
            {
                transaction.Title = formdata.Title;
                transaction.Description = formdata.Description;
                transaction.Amount = formdata.Amount;
                transaction.DateUpdated = DateTime.Now;
                transaction.Date = formdata.Date;
                DbContext.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest("You are not the owner or member of this HouseHold Transaction.");
            }
        }

        public IHttpActionResult Delete(int? id)
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var allhousehold = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == id);
            var categories = DbContext.Categories.FirstOrDefault(p => p.Id == id);
            var account = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);
            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id);
            if (allhousehold == null && account == null)
            {
                return BadRequest("Either the Household or Account is not found.");
            }
            if (user != null)
            {
                if (transaction != null && transaction.Categories.Households.OwnerId == userId || transaction.CreatedById == userId)
                {
                    DbContext.Transactions.Remove(transaction);
                    transaction.BankAccount.Balance -= transaction.Amount;
                    DbContext.SaveChanges();
                }
                else
                {
                    return BadRequest("You have not Permission to delete this Transation.");
                }
            }
            else
            {
                return BadRequest("The Owner or member should have to be login to delete the transation.");
            }
            return Ok();
        }

        [Route("api/Transaction/VoidTransaction/{id}")]
        public IHttpActionResult VoidTransaction(int? id)
        {
            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id);
            var userId = User.Identity.GetUserId();
            if (transaction != null && transaction.Categories.Households.OwnerId == userId || transaction.CreatedById == userId)
            {
                transaction.Void = true;
                transaction.BankAccount.Balance -= transaction.Amount;
                DbContext.SaveChanges();
            }
            else
            {
                return BadRequest("You have no any transaction for Voiding.");
            }
            return Ok("You Void the transaction.");
        }

        [Route("api/Transaction/DisplayTransaction/{id}")]
        [HttpGet]
        public IHttpActionResult DisplayTransaction(int id)
        {
            var userId = User.Identity.GetUserId();

            var transaction = DbContext.Transactions.Where(p => p.BankAccountId == id &&
                (p.BankAccount.Households.OwnerId == userId
                || p.BankAccount.Households.Users.Any(t => t.Id == userId)))
                .Select(m => new TransactionViewModel
                {
                    Id = m.Id,
                    Title = m.Title,
                    Amount = m.Amount,
                    Description = m.Description,
                    DateCreated = m.DateCreated,
                    DateUpdated = m.DateUpdated,
                    Date = m.Date,
                    OwnerEmail = m.CreatedBy.Email,
                    OwnerId = m.CreatedById
                }).ToList();

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

    }
}
