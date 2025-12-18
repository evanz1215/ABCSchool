using Infrastructure.Tenancy;

namespace Infrastructure.OpenApi;

public class TenantHeaderAttribute() : SwaggerHeaderAttribute(
    headerName: TenancyConstants.TenantIdName,
    description: "Enter Your tenant name to access this API.",
    defaultValue: string.Empty,
    isRequired: true)
{
}