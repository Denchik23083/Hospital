using Hospital.Db.Utilities;

namespace Hospital.Core.Utilities
{
    public static class AppRoles
    {
        public const string Admin = nameof(RoleType.Admin);
        public const string Doctor = nameof(RoleType.Doctor);
        public const string Patient = nameof(RoleType.Patient);
        public const string DoctorAdmin = nameof(RoleType.Doctor) + "," + nameof(RoleType.Admin);
        public const string PatientAdmin = nameof(RoleType.Patient) + "," + nameof(RoleType.Admin);
    }
}
