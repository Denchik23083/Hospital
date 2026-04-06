using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Db.EntityConfiguration
{
    public class PatientConfiguation : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(_ => _.Id);

            builder.Property(_ => _.FirstName).IsRequired();
            builder.Property(_ => _.LastName).IsRequired();
            builder.Property(_ => _.BirthDate).IsRequired();
            builder.Property(_ => _.Phone).IsRequired();
            builder.Property(_ => _.GenderType)
                .HasConversion<int>()
                .IsRequired();

            builder.HasOne(_ => _.User)
                .WithOne(_ => _.Patient)
                .HasForeignKey<Patient>(_ => _.UserId);
        }
    }
}
