using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Db.EntityConfiguration
{
    public class DoctorSlotConfiguration : IEntityTypeConfiguration<DoctorSlot>
    {
        public void Configure(EntityTypeBuilder<DoctorSlot> builder)
        {
            builder.HasKey(_ => _.Id);

            builder.Property(_ => _.Date).IsRequired();
            builder.Property(_ => _.StartTime).IsRequired();
            builder.Property(_ => _.EndTime).IsRequired();
            builder.Property(_ => _.IsAvailible).IsRequired();

            builder.HasOne(_ => _.Doctor)
                .WithMany(_ => _.DoctorSlots)
                .HasForeignKey(_ => _.DoctorId);
        }
    }
}
