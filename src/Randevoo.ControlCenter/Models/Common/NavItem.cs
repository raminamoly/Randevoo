using Randevoo.ControlCenter.Models.Auth;

namespace Randevoo.ControlCenter.Models.Common;

public sealed record NavItem(
    string Label,
    string Href,
    string Icon,
    ControlCenterRole[] Roles);
