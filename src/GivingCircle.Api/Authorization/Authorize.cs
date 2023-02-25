using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GivingCircle.Api.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Authorize : Attribute, IAsyncAuthorizationFilter
    {
        private readonly IEnumerable<string> _roles;

        public Authorize(params string[] roles) 
        {
            _roles = roles ?? Enumerable.Empty<string>();
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Construct the required identity roles
            // From the http context grab the route user id and resource id
            var path = context.HttpContext.Request.Path.ToString();
            Console.WriteLine(path);
            var pathItems = path.Split("/");

            foreach ( var item in pathItems )
            {
                Console.WriteLine(item);
            }

            var requestedUserId = path;

            // Get the user's identity roles

            foreach (var role in _roles)
            {
                Console.WriteLine(role);
            }
            // Make sure that they have each of the identity roles required

            // If they don't return forbidden, else authorize success
            context.Result = new ForbidResult();
            return Task.CompletedTask;
        }
    }
}
