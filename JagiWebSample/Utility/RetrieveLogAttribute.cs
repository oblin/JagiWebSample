using Jagi.Helpers;
using Jagi.Mvc;
using JagiWebSample.Models;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace JagiWebSample.Utility
{
    public class RetrieveLogAttribute : ActionFilterAttribute
    {
        private readonly string _descriptionTemplate = "Retrieve {0}.{1} with {2}";

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controller = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            var parameters = filterContext.ActionDescriptor.GetParameters();

            string description = GetDescription(controller, actionName, parameters, HttpContext.Current.Request.QueryString);
            SaveToAccessLog(description, HttpContext.Current.User.Identity);
            base.OnActionExecuted(filterContext);
        }

        private string GetDescription(string controller, string actionName, ParameterDescriptor[] parameters, NameValueCollection query)
        {
            string parameterDesc = string.Empty;

            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType.IsSimple())
                {
                    if (query[parameter.ParameterName] != null)
                    {
                        parameterDesc = parameterDesc + parameter.ParameterName + ":" + query[parameter.ParameterName].ToString() + ";";

                    }
                }
                else {
                    // Issue: 只能處理一層，如果是 multiple object 無法處理
                    foreach (var prop in parameter.ParameterType.GetProperties())
                    {
                        if (query[prop.Name] != null)
                            parameterDesc = parameterDesc + 
                                parameter.ParameterName + ":" + query[prop.Name].ToString() + ";";
                    }
                }
            }

            return _descriptionTemplate.FormatWith(controller, actionName, parameterDesc);
        }

        private string GetUserName(IIdentity identity)
        {
#if DEBUG
            if (!identity.IsAuthenticated || identity.Name == null)
                return "Not Login!";
#else
            if (!identity.IsAuthenticated || identity.Name == null)
                throw new UnauthorizedAccessException(ConstantString.UNAUTHORIZED_ACCESS_EXCEPTION);
#endif
            return identity.Name;
        }

        private void SaveToAccessLog(string description, IIdentity identity)
        {
            DataAccessLog log = new DataAccessLog();
            log.UserName = GetUserName(identity);
            log.ActionDescription = description;
            log.AccessDate = DateTime.Now;

            using (var context = ServiceLocator.Current.GetInstance<DataContext>())
            {
                context.AccessLogs.Add(log);
                context.SaveChanges();
            }
        }
    }
}