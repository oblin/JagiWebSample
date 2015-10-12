using Microsoft.Web.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Jagi.Mvc
{
    public abstract class JagiControllerBase : Controller
    {
        public BetterJsonResult<T> BetterJson<T>(T model)
        {
            return new BetterJsonResult<T>() { Data = model };
        }

        protected ActionResult RedirectToAction<TController>(Expression<Action<TController>> action)
            where TController : Controller
        {
            return ControllerExtensions.RedirectToAction(this, action);
        }

        protected BetterJsonResult JsonValidationError()
        {
            var result = new BetterJsonResult();
            foreach (var validationError in ModelState.Values.SelectMany(s => s.Errors))
            {
                result.AddError(validationError.ErrorMessage);
            }

            return result;
        }

        protected BetterJsonResult JsonError(string errorMessage)
        {
            var result = new BetterJsonResult();
            result.AddError(errorMessage);

            return result;
        }

        protected BetterJsonResult JsonSuccess<T>(T data)
        {
            var result = new BetterJsonResult<T> { Data = data };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        protected BetterJsonResult JsonSuccess()
        {
            var result = new BetterJsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }
    }
}
