
using Hospital.Repositories.PatientRepository;

namespace Hospital.Services.PatientService
{
    public class PatientService(IPatientRepository repository) : IPatientService
    {
        private readonly IPatientRepository _repository = repository;

        public async Task<decimal> GetPatientBalanceAsync(int userId)
        {
            return await _repository.GetPatientBalanceAsync(userId);
        }
    }
}
