using Hospital.Core.Models.Response;
using Hospital.Db;
using Hospital.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories.DoctorRepository
{
    public class DoctorRepository(HospitalContext context) : IDoctorRepository
    {
        private readonly HospitalContext _context = context;

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync(CancellationToken ct)
        {
            return await _context.Doctors
                .Include(_ => _.User)
                .Include(_ => _.Specialty)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsBySpecialtyAsync(int specialtyId, CancellationToken ct)
        {
            return await _context.Doctors
                .Where(_ => _.SpecialtyId == specialtyId)
                .ToListAsync(ct);
        }

        public async Task<Doctor?> GetDoctorAsync(int id, CancellationToken ct)
        {
            return await _context.Doctors
                .Include(_ => _.User)
                .Include(_ => _.Specialty)
                .FirstOrDefaultAsync(_ => _.Id == id, ct);
        }

        public async Task<Doctor?> GetDoctorByUserAsync(int userId, CancellationToken ct)
        {
            return await _context.Doctors
                .Include(_ => _.User)
                .Include(_ => _.Specialty)
                .FirstOrDefaultAsync(_ => _.UserId == userId, ct);
        }

        public async Task CreateDoctorAsync(Doctor doctor, CancellationToken ct)
        {
            await _context.Doctors.AddAsync(doctor, ct);
        }

        public Task DeleteDoctorAsync(Doctor doctor, CancellationToken ct = default)
        {
            _context.Doctors.Remove(doctor);

            return Task.CompletedTask;
        }
    }
}
