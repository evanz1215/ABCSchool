using System.Collections.ObjectModel;

namespace Infrastructure.Constants;

public static class SchoolAction
{
    public const string Read = nameof(Read);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string RefreshToken = nameof(RefreshToken);
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
    public const string Tokens = nameof(Tokens);
}

public record SchoolPermission(string action, string feature, string description, string group, bool isBasic = false, bool isRoot = false)
{
    public string Name => NameFor(action, feature);

    public static string NameFor(string action, string feature) => $"Permission.{feature}.{action}";
}

public static class SchoolPermissions
{
    private const string Tenancy = "Tenancy";
    private const string SystemAccess = "SystemAccess";
    private const string Academics = "Academics";

    private static readonly SchoolPermission[] _allPermissions =
    [
        new SchoolPermission(SchoolAction.Create, SchoolFeature.Tenants, "Create Tenants", Tenancy, isRoot: true),
            new SchoolPermission(SchoolAction.Read, SchoolFeature.Tenants, "Read Tenants", Tenancy, isRoot: true),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.Tenants, "Update Tenants", Tenancy, isRoot: true),
            new SchoolPermission(SchoolAction.UpgradeSubscription, SchoolFeature.Tenants, "Upgrade Tenant's Subscription", Tenancy, isRoot: true),

            new SchoolPermission(SchoolAction.Create, SchoolFeature.Users, "Create Users", SystemAccess),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.Users, "Update Users", SystemAccess),
            new SchoolPermission(SchoolAction.Delete, SchoolFeature.Users, "Delete Users", SystemAccess),
            new SchoolPermission(SchoolAction.Read, SchoolFeature.Users, "Read Users", SystemAccess),

            new SchoolPermission(SchoolAction.Read, SchoolFeature.UserRoles, "Read User Roles", SystemAccess),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.UserRoles, "Update User Roles", SystemAccess),

            new SchoolPermission(SchoolAction.Read, SchoolFeature.Roles, "Read Roles", SystemAccess),
            new SchoolPermission(SchoolAction.Create, SchoolFeature.Roles, "Create Roles", SystemAccess),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.Roles, "Update Roles", SystemAccess),
            new SchoolPermission(SchoolAction.Delete, SchoolFeature.Roles, "Delete Roles", SystemAccess),

            new SchoolPermission(SchoolAction.Read, SchoolFeature.RoleClaims, "Read Role Claims/Permissions", SystemAccess),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.RoleClaims, "Update Role Claims/Permissions", SystemAccess),

            new SchoolPermission(SchoolAction.Read, SchoolFeature.Schools, "Read Schools", Academics, isBasic: true),
            new SchoolPermission(SchoolAction.Create, SchoolFeature.Schools, "Create Schools", Academics),
            new SchoolPermission(SchoolAction.Update, SchoolFeature.Schools, "Update Schools", Academics),
            new SchoolPermission(SchoolAction.Delete, SchoolFeature.Schools, "Delete Schools", Academics),

            new SchoolPermission(SchoolAction.RefreshToken, SchoolFeature.Tokens, "Generate Refresh Token", SystemAccess, isBasic: true)
    ];

    public static IReadOnlyList<SchoolPermission> All { get; }
        = new ReadOnlyCollection<SchoolPermission>(_allPermissions);

    public static IReadOnlyList<SchoolPermission> Root { get; }
        = new ReadOnlyCollection<SchoolPermission>(_allPermissions.Where(x => x.isRoot).ToArray());

    public static IReadOnlyList<SchoolPermission> Admin { get; }
        = new ReadOnlyCollection<SchoolPermission>(_allPermissions.Where(x => !x.isRoot).ToArray());

    public static IReadOnlyList<SchoolPermission> Basic { get; }
        = new ReadOnlyCollection<SchoolPermission>(_allPermissions.Where(x => x.isBasic).ToArray());
}