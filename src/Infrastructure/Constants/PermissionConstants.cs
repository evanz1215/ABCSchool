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
    private static readonly SchoolPermission[] _all =
        [
            new SchoolPermission(SchoolAction.Create, SchoolFeature.Tenants, "Create Tenants",isRoot:true),
            new SchoolPermission(SchoolAction.Read, SchoolFeature.Tenants, "Read Tenants",isRoot:true),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.Tenants, "Update Tenants",isRoot:true),
            new SchoolPermission(SchoolAction.UpgradeSubscription, SchoolFeature.Tenants, "Upgrade Tenant's Subscription ",isRoot:true),
            //new SchoolPermission(SchoolAction.Delete, SchoolFeature.Tenants, "Delete Tenants",isRoot:true),
        ];
}