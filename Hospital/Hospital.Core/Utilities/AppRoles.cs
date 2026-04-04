using Hospital.Db.Utilities;

namespace Hospital.Core.Utilities
{
    public static class AppRoles
    {
        public const string God = nameof(RoleType.God);
        public const string AdminGod = nameof(RoleType.Admin) + "," + nameof(RoleType.God);
        public const string DoctorAdminGod = nameof(RoleType.Doctor) + "," + nameof(RoleType.Admin) + "," + nameof(RoleType.God);
        public const string PatientAdminGod = nameof(RoleType.Patient) + "," + nameof(RoleType.Admin) + "," + nameof(RoleType.God);
        public const string All = nameof(RoleType.God) + "," + nameof(RoleType.Admin) + "," + nameof(RoleType.Doctor) + "," + nameof(RoleType.Patient);
    }
}
