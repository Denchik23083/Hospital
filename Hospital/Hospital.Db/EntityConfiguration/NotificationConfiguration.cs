using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Db.EntityConfiguration
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(_ => _.Id);

            builder.Property(_ => _.Message).IsRequired();
            builder.Property(_ => _.CreatedAt).IsRequired();

            builder.HasOne(_ => _.User)
                .WithMany(_ => _.Notifications)
                .HasForeignKey(_ => _.UserId);
        }
    }
}
