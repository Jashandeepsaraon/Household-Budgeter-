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
    public class CategoriesController : ApiController
    {
        private ApplicationDbContext DbContext;
        public CategoriesController()
        {
            DbContext = new ApplicationDbContext();
        }

        public IHttpActionResult Get(int? id)
        {
            var category = DbContext.Categories.FirstOrDefault(user => user.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(new CategoriesViewModel
            {
                Name = category.Name,
                Id = category.Id,
                Description = category.Description,
                DateCreated = category.DateCreated,
                DateUpdated = category.DateUpdated,
                HouseholdName = category.Households.Name,
                Owner = category.Households.Owner.Email
            });
        }

        public IHttpActionResult Get()
        {
            var model = DbContext
               .Categories
               .Select(p => new CategoriesViewModel
               {
                   Name = p.Name,
                   Id = p.Id,
                   Description = p.Description,
                   DateCreated = p.DateCreated,
                   DateUpdated = p.DateUpdated,
                   HouseholdName = p    .Households.Name,
                   Owner = p.Households.Owner.Email
               }).ToList();
            return Ok(model);
        }

        public IHttpActionResult Post(int? id, CategoriesBindingModel formdata)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var houseHold = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == id);
            if (houseHold == null)
            {
                return BadRequest("There is no household.");
            }
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            if (user != null && houseHold.OwnerId == userId)
            {
                Categories categories = new Categories();
                var userName = User.Identity.Name;
                categories.Name = formdata.Name;
                categories.Description = formdata.Description;
                houseHold.Categories.Add(categories);
                //DbContext.Allhouseholds.Add(categories);
                DbContext.SaveChanges();

                var model = new CategoriesViewModel();
                model.Id = categories.Id;
                model.Name = categories.Name;
                model.Description = categories.Description;
                model.Owner = categories.Households.Owner.Email;
                return Ok(model);
            }
            else
            {
                return BadRequest("You are not the owner of this HouseHold.");
            }        
        }

        public IHttpActionResult Put(int? id, CategoriesBindingModel formdata)
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var household = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == id);
            if (household == null)
            {
                return BadRequest("There is no household.");
            }
            var categories = DbContext.Categories.FirstOrDefault(p => p.Id == id);
            if (user != null && categories.Households.OwnerId == userId && categories != null)
            {
                categories.Name = formdata.Name;
                categories.Description = formdata.Description;
                categories.DateUpdated = DateTime.Now;
                DbContext.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest("You are not the owner of this HouseHold Categories.");
            }
        }

        public IHttpActionResult Delete(int? id)
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var allhousehold = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == id);
            var categories = DbContext.Categories.FirstOrDefault(p => p.Id == id);
            if (user != null)
            {
                if (categories != null && categories.Households.OwnerId == userId)
                {
                    DbContext.Categories.Remove(categories);
                    DbContext.SaveChanges();
                }
                else
                {
                    return BadRequest("You have not Permission to delete this Category.");
                }
            }
            else
            {
                return BadRequest();
            }
            return Ok();
        }

        [Route("api/Categories/DisplayUsers/{id}")]
        [HttpGet]
        public IHttpActionResult DisplayUsers(int id)
        {
            var userId = User.Identity.GetUserId();

            var categories = DbContext.Categories.Where(p => p.HouseholdsId == id &&
                (p.Households.OwnerId == userId
                || p.Households.Users.Any(t => t.Id == userId)))
                .Select(m => new CategoriesViewModel
                {
                    Name = m.Name,
                    Description = m.Description,
                    HouseholdName = m.HouseholdName,
                    DateCreated = m.DateCreated,
                    DateUpdated = m.DateUpdated
                })
                .ToList();

            if (categories == null)
            {
                return NotFound();
            }

            return Ok(categories);
        }
    }
}
