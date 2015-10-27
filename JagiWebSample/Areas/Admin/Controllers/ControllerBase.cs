using Jagi.Mvc;
using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace JagiWebSample.Areas.Admin.Controllers
{
    public class ControllerBase : JagiControllerBase
    {
        /// <summary>
        /// 主要處理在執行中可能發生的 Exceptions，轉換成 Json format 提供給前端
        /// 一般錯誤預設為 406 (NotAcceptable)，資料庫錯誤預設為 409 (Conflict)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="codetoExecute"></param>
        /// <returns></returns>
        protected BetterJsonResult GetJsonResult<T>(Func<T> codetoExecute)
        {
            try
            {
                T result = codetoExecute.Invoke();
                return JsonSuccess(result);
            }
            catch (DbEntityValidationException dbEx)
            {
                var message = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        message += String.Format("Class: {0}, Property: {1}, Error: {2}\n", validationErrors.Entry.Entity.GetType().FullName,
                                      validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return JsonError(message, 409);
            }
            catch (DbUpdateException updExc)
            {
                return JsonError(updExc.InnerException.InnerException.Message, 409);
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        /// <summary>
        /// 主要提供不需要回傳值的 executing code
        /// </summary>
        /// <param name="codetoExecute"></param>
        /// <returns></returns>
        protected BetterJsonResult GetJsonResult(Action codetoExecute)
        {
            try
            {
                codetoExecute.Invoke();
                return JsonSuccess();
            }
            catch (DbEntityValidationException dbEx)
            {
                var message = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        message += String.Format("Class: {0}, Property: {1}, Error: {2}\n", validationErrors.Entry.Entity.GetType().FullName,
                                      validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return JsonError(message, 409);
            }
            catch (DbUpdateException updExc)
            {
                return JsonError(updExc.InnerException.InnerException.Message, 409);
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }
    }
}