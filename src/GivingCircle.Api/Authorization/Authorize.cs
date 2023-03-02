using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
 
namespace GivingCircle.Api.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Authorize : Attribute, IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Construct the required identity roles from the http context
            // Grab the route user id and resource id
            var path = context.HttpContext.Request.Path.ToString();
            var pathItems = path.Split("/");

            // The user id in the route
            var routeUserId = pathItems[3];
            
            // Grab the claims user id
            var requestingUserId = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;

            // Make sure that the claims user id matches the route user id
            // If it doesn't then they are forbidden
            if (requestingUserId != routeUserId)
            {
                context.Result = new ForbidResult();
                return Task.CompletedTask;
            }

            // If they don't return forbidden, else authorize success
            return Task.CompletedTask;
        }
    }
}
