using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EventManagementSystem.AppUI.Filters
{
    public class ParticipantAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            if (session.GetString("UserEmail") == null)
            {
                context.Result = new RedirectToActionResult("Login", "Participant", null);
            }
            base.OnActionExecuting(context);
        }
    }
}
