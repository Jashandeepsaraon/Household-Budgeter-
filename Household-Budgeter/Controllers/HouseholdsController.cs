using Household_Budgeter.Models;
using Household_Budgeter.Models.Domain;
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
            var allhouseholds = DbContext.HouseholdManagement.FirstOrDefault(user => user.Id == id);
            if (allhouseholds == null)
            {
                return NotFound();
            }
            return Ok(allhouseholds);
        }

        public IHttpActionResult Get()
        {
            return Ok(DbContext.HouseholdManagement);
        }

        public IHttpActionResult Post(HouseholdsViewModel formdata)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Households households = new Households();
            households.Name = formdata.Name;
            households.Description = formdata.Description;
            DbContext.HouseholdManagement.Add(households);
            DbContext.SaveChanges();
            return Ok(formdata);
        }

        public IHttpActionResult Put(int? id)
        {
            var allhouseholds = DbContext.HouseholdManagement.FirstOrDefault(p => p.Id == id);
            var model = new HouseholdsViewModel();
            model.Name = allhouseholds.Name;
            model.Description = allhouseholds.Description;              
            return Ok(model);
        }       
    }
}
