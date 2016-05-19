using AutoMapper;
using Jagi.Helpers;
using Jagi.Interface;
using Microsoft.Web.Mvc;
using System;
using System.Collections.Generic;
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
            IEnumerable<string> messages =
                ModelState.Values.SelectMany(s => s.Errors).Select(r => r.ErrorMessage);

            return JsonError(messages, errorCode);
        }

        /// <summary>
        /// 處理錯誤訊息回傳，可以指定 HttpStatusCode，如果不指定，預設為 406 NotAcceptable
        /// </summary>
        /// <param name="errorMessage">需要回傳的錯誤訊息</param>
        /// <param name="errorCode">指定的 status code,不傳入值預設為 406</param>
        /// <returns></returns>
        protected BetterJsonResult JsonError(string errorMessage, int? errorCode = null)
        {
            return JsonError(new string[] { errorMessage }, errorCode);
        }

        protected BetterJsonResult JsonError(IEnumerable<string> messages, int? errorCode = null)
        {
            var result = new BetterJsonResult();
            foreach(var message in messages)
                result.AddError(message);

            Response.StatusCode = ConvertToHttpStatusCode(errorCode);
            // IIS 會在錯誤訊息中使用 HTML 美化頁面，設定 Skip 讓錯誤訊息可以直接使用 json string 傳回
            Response.TrySkipIisCustomErrors = true;     
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

        /// <summary>
        /// 提供任意物件傳回經過 sort & filtering 的結果
        /// </summary>
        /// <typeparam name="T">任意物件</typeparam>
        /// <param name="set">資料內容</param>
        /// <param name="pageInfo">如果沒有，則會產生一個新的 pageSize = 25, pageNumber = 1</param>
        /// <returns></returns>
        protected virtual IEnumerable<T> GetPagedList<T>(IEnumerable<T> set, ref PageInfo pageInfo, out int totalCount)
        {
            if (pageInfo == null || pageInfo.PageNumber == 0 || pageInfo.PageSize == 0)
                pageInfo = new PageInfo { PageSize = 25, PageNumber = 1 };

            if (!string.IsNullOrEmpty(pageInfo.SearchKeyword))
                set = set.ContainsWithFieldName(pageInfo.SearchField, pageInfo.SearchKeyword);
            if (!string.IsNullOrEmpty(pageInfo.SortField))
                set = set.OrderByFieldName(pageInfo.SortField, pageInfo.Sort);
            totalCount = set.Count();
            return TakePagedResult(set, pageInfo);
        }


        protected IEnumerable<T> TakePagedResult<T>(IEnumerable<T> set, PageInfo pageInfo)
        {
            return set.Skip((pageInfo.PageNumber - 1) * pageInfo.PageSize)
                      .Take(pageInfo.PageSize);
        }

        protected virtual PageInfo InitializePageInfo()
        {
            return new PageInfo
            {
                PageNumber = 1,
                PageSize = 25,
                //SortField = "Name"
            };
        }

        protected virtual PagedView GetPagedResult<S, D>(PageInfo pageInfo, IEnumerable<S> hdplans)
            where D : new()
        {
            int count;
            hdplans = GetPagedList(hdplans, ref pageInfo, out count);
            //hdplans = TakePagedResult(hdplans, pageInfo);
            var viewResult = Mapper.Map<IEnumerable<D>>(hdplans);

            return new PagedView
            {
                Data = viewResult as IEnumerable<object>,
                TotalCount = count,
                PageNumber = pageInfo.PageNumber,
                PageSize = pageInfo.PageSize,
                Sort = pageInfo.Sort,
                SortField = pageInfo.SortField
            };
        }
    }
}
