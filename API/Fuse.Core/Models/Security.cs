using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Fuse.Core.Models;

// Security domain models stored in the lightweight JSON data store.

public enum SecurityLevel
{
    None,
    RestrictedEditing,
    FullyRestricted
}

public enum SecurityRole
{
    Reader,
    Admin
}

public record SecuritySettings
{
    public SecuritySettings()
    {
    }

    public SecuritySettings(SecurityLevel level, DateTime updatedAt)
    {
        Level = level;
        UpdatedAt = updatedAt;
    }

    public SecurityLevel Level { get; init; } = SecurityLevel.None;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
}

public record SecurityUser
{
    public SecurityUser()
    {
    }

    public SecurityUser(Guid id, string userName, string passwordHash, string passwordSalt, SecurityRole role, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        UserName = userName;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        Role = role;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public string PasswordSalt { get; init; } = string.Empty;
    public SecurityRole Role { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record SecurityState
{
    public SecurityState()
    {
    }

    public SecurityState(SecuritySettings settings, IReadOnlyList<SecurityUser> users)
    {
        Settings = settings;
        Users = users;
    }

    public SecuritySettings Settings { get; init; } = new(SecurityLevel.None, DateTime.UtcNow);
    public IReadOnlyList<SecurityUser> Users { get; init; } = Array.Empty<SecurityUser>();

    [JsonIgnore]
    public bool RequiresSetup => !Users.Any(u => u.Role == SecurityRole.Admin);
}

public record SecurityUserInfo(Guid Id, string UserName, SecurityRole Role, DateTime CreatedAt, DateTime UpdatedAt);

public record LoginSession(string Token, DateTime ExpiresAt, SecurityUserInfo User);
