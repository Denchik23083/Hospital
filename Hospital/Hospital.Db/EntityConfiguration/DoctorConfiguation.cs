using Hospital.Db.Entities;
using Hospital.Db.Utilities;
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
            builder.Property(_ => _.GenderType)
                .HasConversion<int>()
                .IsRequired();

            builder.HasOne(_ => _.Specialty)
                .WithMany(_ => _.Doctors)
                .HasForeignKey(_ => _.SpecialtyId);

            builder.HasOne(_ => _.User)
                .WithOne()
                .HasForeignKey<Doctor>(_ => _.UserId);

            builder.HasData(new List<Doctor>
            {
                new()
                {
                    Id = 1,
                    FirstName = "Глеб",
                    LastName = "Романенко",
                    ExperienceYears = 2,
                    SpecialtyId = 1,
                    GenderType = GenderType.Male
                },
                new()
                {
                    Id = 2,
                    FirstName = "Семен",
                    LastName = "Лобанов",
                    ExperienceYears = 3,
                    SpecialtyId = 1,
                    GenderType = GenderType.Male
                },
                new()
                {
                    Id = 3,
                    FirstName = "Борис",
                    LastName = "Левин",
                    ExperienceYears = 2,
                    SpecialtyId = 1,
                    GenderType = GenderType.Male
                },
                new()
                {
                    Id = 4,
                    FirstName = "Варвара",
                    LastName = "Черноус",
                    ExperienceYears = 1,
                    SpecialtyId = 1,
                    GenderType = GenderType.Female
                },
                new()
                {
                    Id = 5,
                    FirstName = "Мария",
                    LastName = "Колисниченко",
                    ExperienceYears = 3,
                    SpecialtyId = 2,
                    GenderType = GenderType.Female
                },
                new()
                {
                    Id = 6,
                    FirstName = "Светлана",
                    LastName = "Чернышова",
                    ExperienceYears = 1,
                    SpecialtyId = 2,
                    GenderType = GenderType.Female
                },
                new()
                {
                    Id = 7,
                    FirstName = "Вячеслав",
                    LastName = "Селезнев",
                    ExperienceYears = 5,
                    SpecialtyId = 2,
                    GenderType = GenderType.Male
                },
                new()
                {
                    Id = 8,
                    FirstName = "Станислав",
                    LastName = "Башницен",
                    ExperienceYears = 7,
                    SpecialtyId = 3,
                    GenderType = GenderType.Male
                },
                new()
                {
                    Id = 9,
                    FirstName = "Васелиса",
                    LastName = "Шмид",
                    ExperienceYears = 3,
                    SpecialtyId = 3,
                    GenderType = GenderType.Female
                },
                new()
                {
                    Id = 10,
                    FirstName = "Дарья",
                    LastName = "Зайченко",
                    ExperienceYears = 4,
                    SpecialtyId = 4,
                    GenderType = GenderType.Female
                },
                new()
                {
                    Id = 11,
                    FirstName = "Анатолий",
                    LastName = "Войченко",
                    ExperienceYears = 1,
                    SpecialtyId = 4,
                    GenderType = GenderType.Male
                },
                new()
                {
                    Id = 12,
                    FirstName = "Евгений",
                    LastName = "Шевчук",
                    ExperienceYears = 5,
                    SpecialtyId = 5,
                    GenderType = GenderType.Male
                },
                new()
                {
                    Id = 13,
                    FirstName = "Катерина",
                    LastName = "Главко",
                    ExperienceYears = 2,
                    SpecialtyId = 5,
                    GenderType = GenderType.Female
                },
                new()
                {
                    Id = 14,
                    FirstName = "Елизавета",
                    LastName = "Сидорчук",
                    ExperienceYears = 3,
                    SpecialtyId = 6,
                    GenderType = GenderType.Female
                },
                new()
                {
                    Id = 15,
                    FirstName = "Петр",
                    LastName = "Иващенко",
                    ExperienceYears = 8,
                    SpecialtyId = 6,
                    GenderType = GenderType.Male
                },
                new()
                {
                    Id = 16,
                    FirstName = "Тарас",
                    LastName = "Гайдар",
                    ExperienceYears = 2,
                    SpecialtyId = 7,
                    GenderType = GenderType.Male
                },
                new()
                {
                    Id = 17,
                    FirstName = "Анастасия",
                    LastName = "Громова",
                    ExperienceYears = 5,
                    SpecialtyId = 7,
                    GenderType = GenderType.Female
                },
                new()
                {
                    Id = 18,
                    FirstName = "Вероника",
                    LastName = "Борова",
                    ExperienceYears = 4,
                    SpecialtyId = 8,
                    GenderType = GenderType.Female
                },
                new()
                {
                    Id = 19,
                    FirstName = "Оксана",
                    LastName = "Свиридова",
                    ExperienceYears = 2,
                    SpecialtyId = 9,
                    GenderType = GenderType.Female
                },
                new()
                {
                    Id = 20,
                    FirstName = "Полина",
                    LastName = "Ушакова",
                    ExperienceYears = 3,
                    SpecialtyId = 9,
                    GenderType = GenderType.Female
                },
                new()
                {
                    Id = 21,
                    FirstName = "Денис",
                    LastName = "Никифоров",
                    ExperienceYears = 6,
                    SpecialtyId = 9,
                    GenderType = GenderType.Male
                }
            });
        }
    }
}
