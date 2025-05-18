using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class SessionAuthorizeAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;

        int? giseMemurId = session.GetInt32("GiseMemuruId");
        int? subeId = session.GetInt32("GiseMemuruSubeId");

        // Önbelleği engelle
        context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        context.HttpContext.Response.Headers["Pragma"] = "no-cache";
        context.HttpContext.Response.Headers["Expires"] = "0";

        if (!giseMemurId.HasValue || !subeId.HasValue)
        {
            context.Result = new RedirectToActionResult("Giris", "Kisi", null);
            return;
        }

        base.OnActionExecuting(context);
    }
}
