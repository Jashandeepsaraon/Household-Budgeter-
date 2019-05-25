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
    public class BankAccountController : ApiController
    {
        private ApplicationDbContext DbContext;
        public BankAccountController()
        {
            DbContext = new ApplicationDbContext();
        }

        public IHttpActionResult Get(int? id)
        {
            var account = DbContext.BankAccounts.FirstOrDefault(user => user.Id == id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(new BankAccountViewModel
            {
                Name = account.Name,
                Id = account.Id,
                Description = account.Description,
                DateCreated = account.DateCreated,
                DateUpdated = account.DateUpdated,
                Balance = account.Balance,
                Owner = account.Households.Owner.Email
            });
        }

        public IHttpActionResult Get()
        {
            var model = DbContext
               .BankAccounts
               .Select(p => new BankAccountViewModel
               {
                   Name = p.Name,
                   Id = p.Id,
                   Description = p.Description,
                   DateCreated = p.DateCreated,
                   DateUpdated = p.DateUpdated,
                   Balance = p.Balance,
                   Owner = p.Households.Owner.Email
               }).ToList();
            return Ok(model);
        }

        public IHttpActionResult Post(BankAccountBindingModel formdata)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var houseHold = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == formdata.HouseholdsId);
            if (houseHold == null)
            {
                return BadRequest("There is no household.");
            }
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            if (user != null && houseHold.OwnerId == userId)
            {
                BankAccount account = new BankAccount();
                var userName = User.Identity.Name;
                account.Name = formdata.Name;
                account.Description = formdata.Description;
                account.Balance = 0;
                houseHold.BankAccounts.Add(account);
                //DbContext.Allhouseholds.Add(categories);
                DbContext.SaveChanges();

                var model = new BankAccountViewModel();
                model.Id = account.Id;
                model.Name = account.Name;
                model.Description = account.Description;
                model.Balance = account.Balance;
                model.Owner = account.Households.Owner.Email;
                return Ok(model);
            }
            else
            {
                return BadRequest("You are not the owner of this HouseHold.");
            }
        }

        public IHttpActionResult Put(int? id, BankAccountBindingModel formdata)
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var household = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == id);
            var account = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);
            if (user != null && account.Households.OwnerId == userId && account != null)
            {
                account.Name = formdata.Name;
                account.Description = formdata.Description;
                account.DateUpdated = DateTime.Now;
                DbContext.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest("You are not the owner of this HouseHold BankAccount.");
            }
        }

        public IHttpActionResult Recalculating(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var household = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == id);
            var account = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);
            if (user != null && account != null && account.Households.OwnerId == userId)
            {
                decimal result = 0;
                foreach (var t in account.Transactions)
                {
                    if (!t.Void)
                    {
                        result += t.Amount;
                    }
                }
                //result = account.Transactions.Sum(t => t.Void ? 0 : t.Amount);
                account.Balance = result;
                return Ok("Total Amount");
            }
            else
            {
                if (account == null)
                {
                    return NotFound();
                }
                return Unauthorized();
            }
        }

        [Route("api/BankAccount/DisplayAccounts/{id}")]
        [HttpGet]
        public IHttpActionResult DisplayAccounts(int id)
        {
            var userId = User.Identity.GetUserId();

            var account = DbContext.BankAccounts.Where(p => p.HouseholdsId == id &&
                (p.Households.OwnerId == userId
                || p.Households.Users.Any(t => t.Id == userId)))
                .Select(m => new BankAccountViewModel
                {
                    Name = m.Name,
                    Balance = m.Balance,
                    Description = m.Description,
                    DateCreated = m.DateCreated,
                    DateUpdated = m.DateUpdated
                })
                .ToList();

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

    }
}
