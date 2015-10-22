using System.Web.Mvc;

namespace Jagi.Mvc
{
    /// <summary>
    /// 主要目的：在actions 間，使用 tempdata 移轉相關的警告內容
    /// 透過 ExcecuteResult 方式，將 Alert object 放入 TempData 中
    /// 請搭配 AlertExtension 使用，可以提供 ActionResult extension methods，如：
    ///    return View(result).WithWarning("查詢 {0} 資料表格無資料".FormatWith(tableName));
    /// </summary>
    public class AlertDecoratorResult : ActionResult
    {
        public ActionResult InnerResult { get; set; }
        public string AlertClass { get; set; }
        public string Message { get; set; }

        public AlertDecoratorResult(ActionResult innerResult,
            string alertClass, string message)
        {
            InnerResult = innerResult;
            AlertClass = alertClass;
            Message = message;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var alerts = context.Controller.TempData.GetAlerts();
            alerts.Add(new Alert(AlertClass, Message));
            InnerResult.ExecuteResult(context);
        }
    }
}