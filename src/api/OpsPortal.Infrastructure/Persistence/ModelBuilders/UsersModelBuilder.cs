using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OpsPortal.Domain.Constants;
using OpsPortal.Domain.Entities;

namespace OpsPortal.Infrastructure.Persistence.ModelBuilders;

internal class UsersModelBuilder : IModelBuilder
{
    private IDatabaseProvider _databaseProvider;

    public UsersModelBuilder(IDatabaseProvider databaseProvider)
    {
        _databaseProvider = databaseProvider;
    }

    public void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Identifier).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            //if (_databaseProvider.IsSqlServer)
            //{
            //    entity.Property(e => e.Identifier).HasMaxLength(256).IsUnicode(false);
            //    entity.Property(e => e.Email).HasMaxLength(256).IsUnicode(false);
            //    entity.Property(e => e.DisplayName).HasMaxLength(256);
            //    entity.Property(e => e.ExternalId).HasMaxLength(256).IsUnicode(false);
            //    entity.Property(e => e.IdentityProvider).HasMaxLength(256).IsUnicode(false);
            //    entity.Property(e => e.PasswordHash).HasMaxLength(512).IsUnicode(false);
            //    entity.Property(e => e.PasswordSalt).HasMaxLength(512).IsUnicode(false);
            //}
            //else if (_databaseProvider.IsPostgreSql)
            //{
            //    entity.Property(e => e.Identifier).HasColumnType("varchar(256)");
            //    entity.Property(e => e.Email).HasColumnType("varchar(256)");
            //    entity.Property(e => e.DisplayName).HasColumnType("varchar(256)");
            //    entity.Property(e => e.ExternalId).HasColumnType("varchar(256)");
            //    entity.Property(e => e.IdentityProvider).HasColumnType("varchar(256)");
            //    entity.Property(e => e.PasswordHash).HasColumnType("varchar(512)");
            //    entity.Property(e => e.PasswordSalt).HasColumnType("varchar(512)");
            //}

            entity.HasData(
                User.CreateSystemUser(
                    SystemDefaults.SystemUsers.System.Id,
                    SystemDefaults.SystemUsers.System.Identifier,
                    SystemDefaults.SystemUsers.System.Email,
                    SystemDefaults.SystemUsers.System.DisplayName
                ),
                User.CreateSystemUser(
                    SystemDefaults.SystemUsers.Migration.Id,
                    SystemDefaults.SystemUsers.Migration.Identifier,
                    SystemDefaults.SystemUsers.Migration.Email,
                    SystemDefaults.SystemUsers.Migration.DisplayName
                ),
                User.CreateSystemUser(
                    SystemDefaults.SystemUsers.Scheduler.Id,
                    SystemDefaults.SystemUsers.Scheduler.Identifier,
                    SystemDefaults.SystemUsers.Scheduler.Email,
                    SystemDefaults.SystemUsers.Scheduler.DisplayName
                ));
        });
    }
}
