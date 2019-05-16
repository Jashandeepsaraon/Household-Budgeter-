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
    public class HouseholdsController : ApiController
    {
        private ApplicationDbContext DbContext;
        public HouseholdsController()
        {
            DbContext = new ApplicationDbContext();
        }

        public IHttpActionResult Get(int? id)
        {
            var allhouseholds = DbContext.Allhouseholds.FirstOrDefault(user => user.Id == id);
            if (allhouseholds == null)
            {
                return NotFound();
            }
            return Ok(allhouseholds);
        }

        public IHttpActionResult Get()
        {
            var model = DbContext
               .Allhouseholds
               .Select(p => new HouseholdsViewModel
               {
                   Name = p.Name,
                   Id = p.Id,
                   Description = p.Description,
                   DateCreated = p.DateCreated,
                   DateUpdated = p.DateUpdated,
                   Owner = p.Owner.Email
               }).ToList();
            return Ok(model);
        }


        public IHttpActionResult Post(HouseholdsBindingModel formdata)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            Households households = new Households();
            var userName = User.Identity.Name;
            households.Owner = DbContext.Users.FirstOrDefault(p => p.UserName == userName);
            households.Name = formdata.Name;
            households.Description = formdata.Description;
            DbContext.Allhouseholds.Add(households);
            DbContext.SaveChanges();

            var model = new HouseholdsViewModel();
            model.Id = households.Id;
            model.Name = households.Name;
            model.Description = households.Description;
            model.Owner = households.Owner.Email;
            return Ok(model);
        }

        public IHttpActionResult Put(int? id, HouseholdsBindingModel formdata)
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var household = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == id);
            if (user != null || household.OwnerId == userId)
            {             
                if (household == null || household.OwnerId == userId)
                {
                    return NotFound();
                }
                household.Name = formdata.Name;
                household.Description = formdata.Description;
                household.DateUpdated = DateTime.Now;
                DbContext.SaveChanges();
                return Ok(household);
            }
            else
            {
                return BadRequest("You are not the owner of this Household.");
            }
        }

        public IHttpActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var allhousehold = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == id);
            if (user != null || allhousehold.OwnerId == userId)
            {      
                if (allhousehold != null || allhousehold.OwnerId == userId)
                {
                    DbContext.Allhouseholds.Remove(allhousehold);
                    DbContext.SaveChanges();
                }
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

    }
}


