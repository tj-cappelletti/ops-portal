using System.Security.Cryptography;
using System.Text;
using OpsPortal.Domain.Entities;

namespace OpsPortal.Domain.Constants;

public static class SystemDefaults
{
    /// <summary>
    ///     Base timestamp for all system-seeded data
    ///     Using a memorable date that's clearly before any real data
    /// </summary>
    public static readonly DateTime BaseDateTime = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    ///     Namespace GUID for generating deterministic v5 UUIDs for seed data
    ///     This ensures consistent GUIDs across all environments
    /// </summary>
    private static readonly Guid SeedDataNamespace = new("6ba7b810-9dad-11d1-80b4-00c04fd430c8");

    /// <summary>
    ///     Generate a deterministic GUID based on a string value
    ///     This ensures the same string always produces the same GUID
    /// </summary>
    public static Guid GenerateId(string value)
    {
        // Create a v5 UUID (SHA1 based) - deterministic for the same input
        using var sha1 = SHA1.Create();
        var nameBytes = Encoding.UTF8.GetBytes(value);
        var namespaceBytes = SeedDataNamespace.ToByteArray();

        var hashInput = new byte[namespaceBytes.Length + nameBytes.Length];
        Buffer.BlockCopy(namespaceBytes, 0, hashInput, 0, namespaceBytes.Length);
        Buffer.BlockCopy(nameBytes, 0, hashInput, namespaceBytes.Length, nameBytes.Length);

        var hash = sha1.ComputeHash(hashInput);
        var newGuid = new byte[16];
        Array.Copy(hash, 0, newGuid, 0, 16);

        // Set version (5) and variant bits
        newGuid[6] = (byte)((newGuid[6] & 0x0F) | 0x50);
        newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);

        return new Guid(newGuid);
    }

    public static class SystemUsers
    {
        public static bool IsSeedDataTimestamp(DateTime timestamp)
        {
            return Math.Abs((timestamp - BaseDateTime).TotalSeconds) < 60;
        }

        public static bool IsSystemUser(Guid userId)
        {
            return userId == System.Id ||
                   userId == Migration.Id ||
                   userId == Scheduler.Id;
        }

        public static class System
        {
            public const string DisplayName = "System";
            public const string Email = "system@ops-portal.local";
            public const string Identifier = "system";
            public static readonly DateTime CreatedAt = BaseDateTime;
            public static readonly Guid Id = GenerateId("SystemUser:System");
            public static readonly DateTime UpdatedAt = BaseDateTime;
        }

        public static class Migration
        {
            public const string DisplayName = "Data Migration";
            public const string Email = "migration@ops-portal.local";
            public const string Identifier = "migration";
            public static readonly DateTime CreatedAt = BaseDateTime;
            public static readonly Guid Id = GenerateId("SystemUser:Migration");
            public static readonly DateTime UpdatedAt = BaseDateTime;
        }

        public static class Scheduler
        {
            public const string DisplayName = "Task Scheduler";
            public const string Email = "scheduler@ops-portal.local";
            public const string Identifier = "scheduler";
            public static readonly DateTime CreatedAt = BaseDateTime;
            public static readonly Guid Id = GenerateId("SystemUser:Scheduler");
            public static readonly DateTime UpdatedAt = BaseDateTime;
        }
    }

    public static class Statuses
    {
        // Helper to get all seed statuses
        public static IEnumerable<SeedStatusData> GetAllSeedData()
        {
            yield return Active.ToSeedData();
            yield return Inactive.ToSeedData();
            yield return Deprecated.ToSeedData();
            yield return Maintenance.ToSeedData();
        }

        public static class Active
        {
            public const string Color = "#28a745";
            public const string Description = "Solution stack is active and available for use";
            public const int DisplayOrder = 1;
            public const string Name = "Active";
            public static readonly Guid Id = GenerateId("Status:Active");

            public static SeedStatusData ToSeedData()
            {
                return new SeedStatusData(Id, Name, Description, Color, DisplayOrder);
            }
        }

        public static class Inactive
        {
            public const string Color = "#6c757d";
            public const string Description = "Solution stack is inactive but retained";
            public const int DisplayOrder = 2;
            public const string Name = "Inactive";
            public static readonly Guid Id = GenerateId("Status:Inactive");

            public static SeedStatusData ToSeedData()
            {
                return new SeedStatusData(Id, Name, Description, Color, DisplayOrder);
            }
        }

        public static class Deprecated
        {
            public const string Color = "#ffc107";
            public const string Description = "Solution stack is deprecated and should not be used";
            public const int DisplayOrder = 3;
            public const string Name = "Deprecated";
            public static readonly Guid Id = GenerateId("Status:Deprecated");

            public static SeedStatusData ToSeedData()
            {
                return new SeedStatusData(Id, Name, Description, Color, DisplayOrder);
            }
        }

        public static class Maintenance
        {
            public const string Color = "#fd7e14";
            public const string Description = "Solution stack is under maintenance";
            public const int DisplayOrder = 4;
            public const string Name = "Maintenance";
            public static readonly Guid Id = GenerateId("Status:Maintenance");

            public static SeedStatusData ToSeedData()
            {
                return new SeedStatusData(Id, Name, Description, Color, DisplayOrder);
            }
        }
    }

    public static class Roles
    {
        public static class Admin
        {
            public const string Description = "Full system access";
            public const string Name = "Administrator";
            public static readonly Guid Id = GenerateId("Role:Administrator");
        }

        public static class Operator
        {
            public const string Description = "Can manage solution stacks";
            public const string Name = "Operator";
            public static readonly Guid Id = GenerateId("Role:Operator");
        }

        public static class Viewer
        {
            public const string Description = "Read-only access";
            public const string Name = "Viewer";
            public static readonly Guid Id = GenerateId("Role:Viewer");
        }
    }
}
