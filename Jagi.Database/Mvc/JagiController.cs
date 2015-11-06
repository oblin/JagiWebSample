using Jagi.Interface;
using Jagi.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Web.Mvc;

namespace Jagi.Database.Mvc
{
    public class JagiController : JagiControllerBase
    {
        protected List<PropertyRule> columnsValidateResult = null;
        protected bool ColumnsValid<T>(T entity)
        {
            columnsValidateResult = ColumnsValidations.ColumnsValidate(entity);
            if (columnsValidateResult.Count > 0)
                return false;

            return true;
        }

        protected override BetterJsonResult JsonValidationError(int? errorCode = default(int?))
        {
            var result = new BetterJsonResult();
            foreach (var validationError in ModelState.Values.SelectMany(s => s.Errors))
            {
                result.AddError(validationError.ErrorMessage);
            }

            foreach (var propertyRule in columnsValidateResult)
            {
                string error = string.Empty;
                foreach (var rule in propertyRule.Rules)
                    error = error + rule + rule.Value + '\n';
                if (!string.IsNullOrEmpty(error))
                    result.AddError(error);
            }
            Response.StatusCode = ConvertToHttpStatusCode(errorCode);
            return result;
        }

        /// <summary>
        /// 主要處理在執行中可能發生的 Exceptions，轉換成 Json format 提供給前端
        /// 一般錯誤預設為 406 (NotAcceptable)，資料庫錯誤預設為 409 (Conflict)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="codetoExecute"></param>
        /// <param name="isCamelCase">是否轉換小寫，預設 true </param>
        /// <returns></returns>
        protected BetterJsonResult GetJsonResult<T>(Func<T> codetoExecute, bool isCamelCase = true)
        {
            try
            {
                T result = codetoExecute.Invoke();
                return JsonSuccess(result, isCamelCase);
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

        protected ActionResult GetActionResult(Func<ActionResult> codetoExecute, string message = null)
        {
            try
            {
                var result = codetoExecute.Invoke();
                message = message ?? "處理作業成功！";
                return result.WithSuccess(message);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        message += String.Format("Class: {0}, Property: {1}, Error: {2}\n", validationErrors.Entry.Entity.GetType().FullName,
                                      validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                ViewBag.ErrorMessage = message;
                return View("Error");
            }
            catch (DbUpdateException updExc)
            {
                ViewBag.ErrorMessage = updExc.InnerException.InnerException.Message;
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        protected IEnumerable<T> GetPagedSize<T>(IEnumerable<T> set, PageInfo pageInfo)
        {
            return set.Skip((pageInfo.PageNumber - 1) * pageInfo.PageSize)
                      .Take(pageInfo.PageSize);
        }
    }
}