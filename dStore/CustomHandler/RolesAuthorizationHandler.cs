using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace eStore.CustomHandler
{
    public class RolesAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            bool validRole = false;
            if (requirement.AllowedRoles == null || !requirement.AllowedRoles.Any())
            {
                validRole = false;
            }
            else
            {
                var claims = context.User.Claims;
                var role = claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Role));
                if (role != null)
                {
                    var roles = requirement.AllowedRoles;
                    if (role.Value.Equals("Admin") && roles.Contains(role.Value))
                    {
                        validRole = true;
                    }
                    else if (!role.Value.Equals("Admin") && roles.Contains("User"))
                    {
                        validRole = true;
                    }
                    else
                    {
                        validRole = false;
                    }
                }
            }

            if (validRole)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
