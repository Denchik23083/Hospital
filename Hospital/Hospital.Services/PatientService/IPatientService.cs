namespace Hospital.Services.PatientService
{
    public interface IPatientService
    {
        Task<decimal> GetPatientBalanceAsync(int userId);
    }
}