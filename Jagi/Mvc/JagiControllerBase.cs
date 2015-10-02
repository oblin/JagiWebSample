using System.Web.Mvc;

namespace Jagi.Mvc
{
    public abstract class JagiControllerBase : Controller
    {
        public BetterJsonResult<T> BetterJson<T>(T model)
        {
            return new BetterJsonResult<T>() { Data = model };
        }
    }
}
