﻿using Microsoft.Web.Mvc;
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

        /// <summary>
        /// 當 ModelStatus.IsValid = false 時候，可以自動處理 ModelStatus.Errors 將之轉成錯誤訊息回傳
        /// </summary>
        /// <param name="errorCode">指定的 status code, 不傳入值預設為 406</param>
        /// <returns></returns>
        protected virtual BetterJsonResult JsonValidationError(int? errorCode = null)
        {
            var result = new BetterJsonResult();
            foreach (var validationError in ModelState.Values.SelectMany(s => s.Errors))
            {
                result.AddError(validationError.ErrorMessage);
            }

            Response.StatusCode = ConvertToHttpStatusCode(errorCode);
            return result;
        }

        /// <summary>
        /// 處理錯誤訊息回傳，可以指定 HttpStatusCode，如果不指定，預設為 406 NotAcceptable
        /// </summary>
        /// <param name="errorMessage">需要回傳的錯誤訊息</param>
        /// <param name="errorCode">指定的 status code,不傳入值預設為 406</param>
        /// <returns></returns>
        protected BetterJsonResult JsonError(string errorMessage, int? errorCode = null)
        {
            var result = new BetterJsonResult();
            result.AddError(errorMessage);

            Response.StatusCode = ConvertToHttpStatusCode(errorCode);
            return result;
        }

        protected BetterJsonResult JsonSuccess<T>(T data, bool isCamelCase = true)
        {
            var result = new BetterJsonResult<T> { Data = data, IsCamelCase = isCamelCase };
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            Response.StatusCode = (int)HttpStatusCode.OK;
            return result;
        }

        protected BetterJsonResult JsonSuccess()
        {
            var result = new BetterJsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            Response.StatusCode = (int)HttpStatusCode.OK;
            return result;
        }

        /// <summary>
        /// 處理 Exceptions 回傳 string message，這當 MVC 轉 JsonResult 時候很重要
        /// 因為不會有任何的錯誤訊息。
        /// 請注意，傳入的 codeToExcute 請直接設定 物件即可，內部會包裝成 JsonSuccess
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="codetoExecute">Function method</param>
        /// <param name="errorCode">指定的 status code, 不傳入值預設為 406</param>
        /// <returns>會將物件轉成 BetterJsonResult object</returns>
        protected BetterJsonResult GetJsonResult<T>(Func<T> codetoExecute, int? errorCode = null)
        {
            try
            {
                T result = codetoExecute.Invoke();
                if (result is BetterJsonResult)
                    throw new ArgumentException("請勿使用 JsonSuccess 包裝回傳成: JsonResult，內部會自動包裝");

                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message, errorCode);
            }
        }

        protected int ConvertToHttpStatusCode(int? errorCode)
        {
            HttpStatusCode result;
            if (errorCode != null && Enum.IsDefined(typeof(HttpStatusCode), errorCode))
            {
                if (!Enum.TryParse(errorCode.ToString(), out result))
                    result = HttpStatusCode.NotAcceptable;
            }
            else
                result = HttpStatusCode.NotAcceptable;
            return (int)result;
        }
    }
}
