using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Db
{
    public class HospitalContext(DbContextOptions<HospitalContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Specialty> Specialties { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<DoctorSlot> DoctorSlots { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
