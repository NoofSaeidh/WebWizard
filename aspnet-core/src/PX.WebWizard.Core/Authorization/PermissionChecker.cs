using Abp.Authorization;
using PX.WebWizard.Authorization.Roles;
using PX.WebWizard.Authorization.Users;

namespace PX.WebWizard.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
