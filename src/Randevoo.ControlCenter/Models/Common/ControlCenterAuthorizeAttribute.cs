using Randevoo.ControlCenter.Models.Auth;

namespace Randevoo.ControlCenter.Models.Common;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class ControlCenterAuthorizeAttribute : Attribute
{
    public ControlCenterAuthorizeAttribute(params ControlCenterRole[] roles)
    {
        Roles = roles;
    }

    public IReadOnlyCollection<ControlCenterRole> Roles { get; }
}
