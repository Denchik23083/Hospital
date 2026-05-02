using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Db.EntityConfiguration
{
    public class DoctorConfiguation : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(_ => _.Id);

            builder.Property(_ => _.FirstName).IsRequired();
            builder.Property(_ => _.LastName).IsRequired();
            builder.Property(_ => _.ExperienceYears).IsRequired();
            builder.Property(_ => _.WorkDayStart).IsRequired();
            builder.Property(_ => _.WorkDayEnd).IsRequired();
            builder.Property(_ => _.GenderType)
                .HasConversion<int>()
                .IsRequired();

            builder.HasOne(_ => _.Specialty)
                .WithMany(_ => _.Doctors)
                .HasForeignKey(_ => _.SpecialtyId);

            builder.HasOne(_ => _.User)
                .WithOne(_ => _.Doctor)
                .HasForeignKey<Doctor>(_ => _.UserId);
        }
    }
}
