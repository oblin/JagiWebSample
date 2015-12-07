using Jagi.Database.Cache;
using Jagi.Interface;
using Jagi.Helpers;
using Jagi.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

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
            if (columnsValidateResult != null)
            {
                foreach (var propertyRule in columnsValidateResult)
                {
                    string error = string.Empty;
                    foreach (var rule in propertyRule.Rules)
                        error = error + rule + rule.Value + '\n';
                    if (!string.IsNullOrEmpty(error))
                        result.AddError(error);
                }
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

        protected override PagedView GetPagedResult<S, D>(PageInfo pageInfo, IEnumerable<S> itemSet)
        {
            var result = base.GetPagedResult<S, D>(pageInfo, itemSet);
            result.Headers = EntityHelper.GetDisplayName(new D());

            return result;
        }

        protected PagedView GetPagedResultWithDecoding<S, D>(PageInfo pageInfo, IEnumerable<S> itemSet)
            where D : new()
        {
            var result = GetPagedResult<S, D>(pageInfo, itemSet);
            result.Headers = EntityHelper.GetDisplayName(new D());
            IEnumerable<object> list = DecodeByCodeDetails(result.Data);
            result.Data = list;
            return result;
        }

        #region Failed 因為計算邏輯有問題，會取出 parent code 轉換後的值，而不是轉換前
        //private IEnumerable<object> DecodeByCodeDetails_Failed(IEnumerable<object> enumerable)
        //{
        //    if (enumerable.Count() == 0)
        //        return enumerable;
        //    var columns = new ColumnsCache();
        //    var first = enumerable.First();
        //    string typeName = first.GetType().Name;
        //    string tableName = columns.GetRelativeTableName(typeName);
        //    if (string.IsNullOrEmpty(tableName))
        //        return enumerable;

        //    CodeCache codes = new CodeCache();
        //    foreach (var property in first.GetType().GetProperties())
        //    {
        //        var column = columns.Get(tableName, property.Name);
        //        if (column == null || string.IsNullOrEmpty(column.DropdwonKey))
        //            continue;
        //        foreach (var item in enumerable)
        //        {
        //            var originItem = (JObject)item.CloneJson();
        //            var value = property.GetGetMethod().Invoke(item, null);
        //            string parentCode = null;
        //            if (!string.IsNullOrEmpty(column.DropdwonCascade))
        //            {
        //                var parentProperty = first.GetType().GetProperties()
        //                    .FirstOrDefault(p => p.Name == column.DropdwonCascade);
        //                if (parentProperty != null)
        //                {
        //                    var parentValue = originItem.GetValue(column.DropdwonCascade).Value<string>();
        //                    //var parentValue = parentProperty.GetGetMethod().Invoke(originItem, null);
        //                    if (!string.IsNullOrEmpty(parentValue))
        //                        parentCode = parentValue.ToString();
        //                }

        //            }
        //            var desc = codes.GetCodeDesc(column.DropdwonKey, value.ToString(), parentCode);
        //            if (!string.IsNullOrEmpty(desc))
        //                property.GetSetMethod().Invoke(item, new object[] { desc });
        //        }
        //    }

        //    return enumerable;
        //}
        #endregion

        private IEnumerable<object> DecodeByCodeDetails(IEnumerable<object> enumerable)
        {
            if (enumerable.Count() == 0)
                return enumerable;
            var columns = new ColumnsCache();
            var first = enumerable.First();
            string typeName = first.GetType().Name;
            string tableName = columns.GetRelativeTableName(typeName);
            if (string.IsNullOrEmpty(tableName))
                return enumerable;

            CodeCache codes = new CodeCache();
            foreach (var item in enumerable)
            {
                var originItem = (JObject)item.CloneJson();

                foreach (var property in item.GetType().GetProperties())
                {
                    var column = columns.Get(tableName, property.Name);
                    if (column == null || string.IsNullOrEmpty(column.DropdwonKey))
                        continue;
                    var value = property.GetGetMethod().Invoke(item, null);
                    if (value == null)
                        continue;
                    string parentCode = null;
                    if (!string.IsNullOrEmpty(column.DropdwonCascade))
                    {
                        var parentProperty = first.GetType().GetProperties()
                            .FirstOrDefault(p => p.Name == column.DropdwonCascade);
                        if (parentProperty != null)
                        {
                            var parentValue = originItem.GetValue(column.DropdwonCascade).Value<string>();
                            //var parentValue = parentProperty.GetGetMethod().Invoke(originItem, null);
                            if (!string.IsNullOrEmpty(parentValue))
                                parentCode = parentValue.ToString();
                        }

                    }
                    var desc = codes.GetCodeDesc(column.DropdwonKey, value.ToString(), parentCode);
                    if (!string.IsNullOrEmpty(desc))
                        property.GetSetMethod().Invoke(item, new object[] { desc });
                }
            }

            return enumerable;
        }
    }
}