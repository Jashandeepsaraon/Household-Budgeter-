using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class Invitation
    {
        public static void Send(string email,string body, string subject)
        {
            var emailservice = new EmailService();
            emailservice.Send(email, body, subject);
        }
    }
}