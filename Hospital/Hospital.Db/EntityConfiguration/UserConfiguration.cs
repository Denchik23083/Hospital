using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Db.EntityConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(_ => _.Id);

            builder.Property(_ => _.PasswordHash).IsRequired();
            builder.Property(_ => _.RefreshToken);
            builder.Property(_ => _.RefreshTokenExpiryTime);
            builder.Property(_ => _.Price)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .IsRequired();
            builder.Property(_ => _.RoleType)
                .HasConversion<int>()
                .IsRequired();
            builder.HasIndex(_ => _.Email)
                .IsUnique();
        }
    }
}
