namespace Infrastructure.Constants;

public static class SchoolAction
{
    public const string Read = nameof(Read);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string UpgradeSubscription = nameof(UpgradeSubscription);
}

public static class SchoolFeature
{
    public const string Tenants = nameof(Tenants);
    public const string Users = nameof(Users);
    public const string Roles = nameof(Roles);
    public const string UserRoles = nameof(UserRoles);
    public const string RoleClaims = nameof(RoleClaims);
    public const string Schools = nameof(Schools);
}

public record SchoolPermission(string action, string feature, string description, bool isBasic = false, bool isRoot = false)
{
    public string Name => NameFor(action, feature);

    public static string NameFor(string action, string feature) => $"Permission.{feature}.{action}";
}

public static class SchoolPermissions
{
    private static readonly SchoolPermission[] _allPermission =
        [
            new SchoolPermission(SchoolAction.Create, SchoolFeature.Tenants, "Create Tenants",isRoot:true),
            new SchoolPermission(SchoolAction.Read, SchoolFeature.Tenants, "Read Tenants",isRoot:true),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.Tenants, "Update Tenants",isRoot:true),
            new SchoolPermission(SchoolAction.UpgradeSubscription, SchoolFeature.Tenants, "Upgrade Tenant's Subscription",isRoot:true),            

            new SchoolPermission(SchoolAction.Create, SchoolFeature.Users, "Create Users"),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.Users, "Update Users"),
            new SchoolPermission(SchoolAction.Delete, SchoolFeature.Users, "Delete Users"),
            new SchoolPermission(SchoolAction.Read, SchoolFeature.Users, "Read Users"),

            new SchoolPermission(SchoolAction.Read, SchoolFeature.UserRoles, "Read UserRoles"),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.UserRoles, "Update UserRoles"),

            new SchoolPermission(SchoolAction.Read, SchoolFeature.Roles, "Read Roles"),
            new SchoolPermission(SchoolAction.Create, SchoolFeature.Roles, "Create Roles"),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.Roles, "Update Roles"),
            new SchoolPermission(SchoolAction.Delete, SchoolFeature.Roles, "Delete Roles"),

            new SchoolPermission(SchoolAction.Read, SchoolFeature.RoleClaims, "Read Role Claims/Permission"),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.RoleClaims, "Update Role Claims/Permission"),

            new SchoolPermission(SchoolAction.Read, SchoolFeature.Schools, "Read Schools"),
            new SchoolPermission(SchoolAction.Create, SchoolFeature.Schools, "Create Schools"),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.Schools, "Update Schools"),
            new SchoolPermission(SchoolAction.Delete, SchoolFeature.Schools, "Delete Schools"),
        ];
}