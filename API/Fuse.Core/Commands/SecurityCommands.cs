using System;
using Fuse.Core.Models;

namespace Fuse.Core.Commands;

public record UpdateSecuritySettings(SecurityLevel Level)
{
    public Guid? RequestedBy { get; init; }
}

public record CreateSecurityUser(string UserName, string Password, SecurityRole Role)
{
    public Guid? RequestedBy { get; init; }
}

public record LoginSecurityUser(string UserName, string Password);

public record LogoutSecurityUser(string Token);
