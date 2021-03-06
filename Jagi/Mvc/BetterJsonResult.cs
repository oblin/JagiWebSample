using Jagi.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Jagi.Mvc
{
    public class BetterJsonResult : JsonResult
    {
        public IList<string> ErrorMessages { get; private set; }

        private bool _isCamelCase = true;
        public bool IsCamelCase { get { return _isCamelCase; } set { _isCamelCase = value; } }

        public BetterJsonResult()
        {
            ErrorMessages = new List<string>();
        }

        public void AddError(string errorMessage)
        {
            ErrorMessages.Add(errorMessage);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            DoUninterestingBaseClassStuff(context);

            SerializeData(context.HttpContext.Response);
        }

        private void DoUninterestingBaseClassStuff(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
        }

        protected virtual void SerializeData(HttpResponseBase response)
        {
            if (ErrorMessages.Any())
            {
                // 繼承 JsonResult.Data，這裡代表如果有任何錯誤訊息，則重新產生 Data object 回傳
                Data = new      
                {
                    ErrorMessage = string.Join("\n", ErrorMessages),
                    ErrorMessages = ErrorMessages.ToArray()
                };
                if (response.StatusCode == 0)
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            if (Data == null) return;

            response.Write(Data.ToJson(isCamelCase: IsCamelCase));
        }
    }

    public class BetterJsonResult<T> : BetterJsonResult
    {
        public new T Data
        {
            get { return (T)base.Data; }
            set { base.Data = value; }
        }
    }
}
