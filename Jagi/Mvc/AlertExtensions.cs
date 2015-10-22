using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Jagi.Mvc
{
    /// <summary>
    /// 將錯誤訊息寫入到 Tempdata 中，方便讓前端可以直接存取顯示（也可以在 actions 間移轉資料）
    /// 前台可以搭配 _Alerts.cshtml 透過 javascript 使用 window.alerts 顯示：
    ///     @Html.Partial("_Alerts") : 放在 _layout.cshtml 可以讓所有 page 存取
    /// </summary>
    public static class AlertExtensions
    {
        const string Alerts = "_Alerts";

        public static List<Alert> GetAlerts(this TempDataDictionary tempData)
        {
            if (!tempData.ContainsKey(Alerts))
                tempData[Alerts] = new List<Alert>();

            return (List<Alert>)tempData[Alerts];
        }

        public static ActionResult WithSuccess(this ActionResult result, string message)
        {
            return new AlertDecoratorResult(result, "alert-success", message);
        }

        public static ActionResult WithInfo(this ActionResult result, string message)
        {
            return new AlertDecoratorResult(result, "alert-info", message);
        }

        public static ActionResult WithWarning(this ActionResult result, string message)
        {
            return new AlertDecoratorResult(result, "alert-warning", message);
        }

        public static ActionResult WithError(this ActionResult result, string message)
        {
            return new AlertDecoratorResult(result, "alert-danger", message);
        }
    }
}
