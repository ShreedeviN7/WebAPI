using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
namespace WebAPITemplate.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var controllerName = context.HttpContext.GetRouteData().Values["controller"];
            var actionName = context.HttpContext.GetRouteData().Values["action"];
            try
            {
                var user = context.HttpContext.Items["User"] != null ? Convert.ToInt64(context.HttpContext.Items["User"]) : 0;
                if (user <= 0)
                {
                    // not logged in
                    context.Result = new JsonResult(new { message = "unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
            catch (Exception ex)
            {
                string ExceptionMessage = string.Empty;
                if (ex.InnerException == null)
                {
                    ExceptionMessage = ex.Message.ToString();
                }
                else
                {
                    ExceptionMessage = ex.InnerException.Message.ToString();
                }
                context.Result = new JsonResult(new { message = "Exception : " + controllerName +"/" + actionName + ": " + ExceptionMessage }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
