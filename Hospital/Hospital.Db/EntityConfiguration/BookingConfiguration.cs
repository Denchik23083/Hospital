using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Db.EntityConfiguration
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(_ => _.Id);

            builder.Property(_ => _.CreatedAt).IsRequired();
            builder.Property(_ => _.BookingStatus)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.HasOne(_ => _.DoctorSlot)
                .WithOne(_ => _.Booking)
                .HasForeignKey<Booking>(_ => _.DoctorSlotId);

            builder.HasOne(_ => _.Patient)
                .WithMany(_ => _.Bookings)
                .HasForeignKey(_ => _.PatientId);
        }
    }
}
