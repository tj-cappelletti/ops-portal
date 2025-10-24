using OpsPortal.Application.Common;
using OpsPortal.Domain.Constants;

namespace OpsPortal.Application.Auditing;

public record ActionContext(
    ActorInfo Actor,
    string? IpAddress = null,
    string? UserAgent = null,
    string? SessionId = null,
    bool IsSystemAction = false,
    Dictionary<string, object>? AdditionalData = null,
    DateTime Timestamp = default)
{
    public DateTime Timestamp { get; init; } = Timestamp == default ? DateTime.UtcNow : Timestamp;

    public static ActionContext CreateSystemActionContext(SystemUserType systemUserType, string reason = "System Action")
    {
        var actorInfo = systemUserType switch
        {
            SystemUserType.System => new ActorInfo(
                SystemDefaults.SystemUsers.System.Id,
                SystemDefaults.SystemUsers.System.DisplayName,
                SystemDefaults.SystemUsers.System.Identifier,
                SystemDefaults.SystemUsers.System.Email),

            SystemUserType.Migration => new ActorInfo(
                SystemDefaults.SystemUsers.Migration.Id,
                SystemDefaults.SystemUsers.Migration.DisplayName,
                SystemDefaults.SystemUsers.Migration.Identifier,
                SystemDefaults.SystemUsers.Migration.Email),

            SystemUserType.Scheduler => new ActorInfo(
                SystemDefaults.SystemUsers.Scheduler.Id,
                SystemDefaults.SystemUsers.Scheduler.DisplayName,
                SystemDefaults.SystemUsers.Scheduler.Identifier,
                SystemDefaults.SystemUsers.Scheduler.Email),

            SystemUserType.Unknown => new ActorInfo(
                SystemDefaults.SystemUsers.Unknown.Id,
                SystemDefaults.SystemUsers.Unknown.DisplayName,
                SystemDefaults.SystemUsers.Unknown.Identifier,
                SystemDefaults.SystemUsers.Unknown.Email),

            _ => new ActorInfo(
                SystemDefaults.SystemUsers.Unknown.Id,
                SystemDefaults.SystemUsers.Unknown.DisplayName,
                SystemDefaults.SystemUsers.Unknown.Identifier,
                SystemDefaults.SystemUsers.Unknown.Email)
        };

        var additionalData = new Dictionary<string, object>
        {
            { "SystemUserType", systemUserType.ToString() },
            { "Reason", reason }
        };

        return new ActionContext(
            actorInfo,
            IsSystemAction: true,
            AdditionalData: additionalData);
    }
}
