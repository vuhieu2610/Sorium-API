using System;
using System.Web.Http.Filters;
namespace Project2.CustomAttributes
{
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response != null)
                actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Methods", "*");
                actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Headers", "*");

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}