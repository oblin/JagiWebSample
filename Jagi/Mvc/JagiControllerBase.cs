using Microsoft.Web.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
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
            result.ErrorStatus = HttpStatusCode.NotAcceptable;
            foreach (var validationError in ModelState.Values.SelectMany(s => s.Errors))
            {
                result.AddError(validationError.ErrorMessage);
            }

            return result;
        }

        protected BetterJsonResult JsonError(string errorMessage)
        {
            var result = new BetterJsonResult();
            result.ErrorStatus = HttpStatusCode.NotAcceptable;
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

        /// <summary>
        /// 處理 Exceptions 回傳 string message，這當 MVC 轉 JsonResult 時候很重要
        /// 因為不會有任何的錯誤訊息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="codetoExecute">Function method</param>
        /// <returns>會將物件轉成 BetterJsonResult object</returns>
        protected BetterJsonResult ExecuteExceptionHandler<T>(Func<T> codetoExecute)
        {
            try
            {
                T result = codetoExecute.Invoke();
                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }
    }
}
