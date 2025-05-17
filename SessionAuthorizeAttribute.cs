using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebBank.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            int? kullaniciId = session.GetInt32("KullaniciId");
            string subeAd = session.GetString("SubeAd");

            // ❗ Tarayıcı önbelleğini engelle
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            context.HttpContext.Response.Headers["Expires"] = "0";

            if (!kullaniciId.HasValue || string.IsNullOrEmpty(subeAd))
            {
                context.Result = new RedirectToActionResult("GiseGiris", "Calisanlar", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
