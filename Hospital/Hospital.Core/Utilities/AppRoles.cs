using Hospital.Db.Utilities;

namespace Hospital.Core.Utilities
{
    public static class AppRoles
    {
        public const string God = nameof(RoleType.God);
        public const string All = nameof(RoleType.God) + "," + nameof(RoleType.Admin) + "," + nameof(RoleType.User);
        public const string GodAdmin = nameof(RoleType.God) + "," + nameof(RoleType.Admin);
    }
}
