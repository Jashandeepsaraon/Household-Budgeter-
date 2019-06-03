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
    public class HouseholdsController : ApiController
    {       
        private ApplicationDbContext DbContext;
        public HouseholdsController()
        {
            DbContext = new ApplicationDbContext();
        }

        public IHttpActionResult Get(int? id)
        {
            var households = DbContext.Allhouseholds.FirstOrDefault(user => user.Id == id);
            if (households == null)
            {
                return NotFound();
            }
            return Ok(new HouseholdsViewModel
            {
                Name = households.Name,
                Id = households.Id,
                Description = households.Description,
                DateCreated = households.DateCreated,
                DateUpdated = households.DateUpdated,
                Owner = households.Owner.Email,

            });
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
                   Owner = p.Owner.Email,
                   //Categories = p.Categories.Select(m => new CategoriesViewModel
                   //{
                   //    Name = m.Name,
                   //    Description = m.Description,
                   //    DateCreated = m.DateCreated,
                   //    DateUpdated = m.DateUpdated,
                   //    Id = m.Id,
                   //}).ToList()
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
            if (household == null)
            {
                return BadRequest("There is no Household to Editing.");
            }
            if (user != null && household.OwnerId == userId)
            {               
                household.Name = formdata.Name;
                household.Description = formdata.Description;
                household.DateUpdated = DateTime.Now;
                DbContext.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest("You are not the owner of this Household.");
            }
        }

        public IHttpActionResult Delete(int? id)
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var allhousehold = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == id);
            if (user != null)
            {
                if (allhousehold == null)
                {
                    return NotFound();
                }
                else if (allhousehold != null && allhousehold.OwnerId == userId)
                {
                    DbContext.Categories.RemoveRange(allhousehold.Categories);
                    DbContext.Allhouseholds.Remove(allhousehold);
                    DbContext.SaveChanges();
                }
                else if (user.ParticipateHouseHold.Any(p => p.Id == allhousehold.Id))
                {
                    allhousehold.Users.Remove(user);
                    DbContext.SaveChanges();
                }
            }
            else
            {
                return BadRequest("You have not Permission to delete this Household.");
            }
            return Ok();
        }

        [Route("api/Households/InviteUsers/{id}")]
        [HttpPost]
        public IHttpActionResult InviteUsers(int? id, string email)
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var userEmail = DbContext.Users.FirstOrDefault(p => p.Email == email);
            var selectedHouse = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == id);
            if (selectedHouse == null)
            {
                return BadRequest("There is no Household to Invite Members.");
            }
            if (user != null && selectedHouse.OwnerId == userId)
            {
                if (!selectedHouse.InviteUsers.Contains(userEmail))
                {
                    selectedHouse.InviteUsers.Add(userEmail);
                    if (userEmail != null)
                    {
                        Invitation.Send(userEmail.Email, $"You are invite to {selectedHouse.Name}. Would you like to Accept invitation?", "Inviation For Household.");
                    }
                    else
                    {
                        return BadRequest("There is no member exist to invite in this Household");
                    }
                    if (userEmail.Id == userId)
                    {
                        ModelState.AddModelError("", "You cannot invite yourself to your own household");
                        return BadRequest(ModelState);
                    }
                }
                DbContext.SaveChanges();
            }
            return Ok("You are successful to invite the user.");
        }

        [Route("api/Households/JoinHousehold/{id}")]
        [HttpPost]
        public IHttpActionResult JoinHousehold(int? id)
        {
            var house = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == id);
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            if (!house.InviteUsers.Any(p => p.Id == userId))
            {
                ModelState.AddModelError("", "You are not invited to this household");
                return BadRequest(ModelState);
            }
            if (!house.Users.Contains(user))
            {
                house.Users.Add(user);
                house.InviteUsers.Remove(user);
            }
            else
            {
                return BadRequest("You are the owner of this household.");
            }
           
            DbContext.SaveChanges();
            return Ok("You are successfully Join the Household.");
        }

        [Route("api/Households/DisplayUsers/{id}")]
        [HttpGet]
        public IHttpActionResult DisplayUsers(int? id)
        {
            var house = DbContext.Allhouseholds.FirstOrDefault(p => p.Id == id);
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);
            var userList = DbContext.Allhouseholds
                             .Where(p => p.Id == id)
                             .Select(p => new DisplayUsersViewModel
                             {                                
                                 Id = p.Id,
                                 Users = p.Users.Select(m => m.Email).ToList()                            
                             });
            return Ok(userList);
        }
    }
}


