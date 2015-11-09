using JagiWebSample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Web;

namespace JagiWebSample.Utility
{
    public class CurrentRequest
    {
        public ApplicationUser User
        {
            get
            {
                var id = System.Web.HttpContext.Current.User.Identity.GetUserId();
                return System.Web.HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>().FindById(id);
            }
        }

        public bool IsAuthenticated
        {
            get { return System.Web.HttpContext.Current.User.Identity.IsAuthenticated; }
        }
    }
}