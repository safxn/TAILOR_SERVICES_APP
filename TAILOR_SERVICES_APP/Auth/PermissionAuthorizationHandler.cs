using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TAILOR_SERVICES_APP.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public PermissionAuthorizationHandler(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (!context.User.Identity?.IsAuthenticated ?? true)
                return;

            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            var userId = _userManager.GetUserId(context.User);
            if (userId == null)
                return;

            bool hasPermission = await _db.EmployeePermissions
                .Include(ep => ep.Permission)
                .AnyAsync(ep => ep.UserId == userId && ep.Permission.Name == requirement.PermissionName);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}