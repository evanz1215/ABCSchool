using Applocation.Features.Tenancy;
using Applocation.Features.Tenancy.Commands;
using Applocation.Features.Tenancy.Queries;
using Infrastructure.Constants;
using Infrastructure.Identity.Auth;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class TenantsController : BaseApiController
{
    [HttpPost("add")]
    [ShouldHavePermission(SchoolAction.Create, SchoolFeature.Tenants)]
    public async Task<IActionResult> CreateTenantAsync([FromBody] CreateTenantRequest createTenantRequest)
    {
        var response = await Sender.Send(new CreateTenantCommand
        {
            CreateTenant = createTenantRequest
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpPost("{tenantId}/activate")]
    [ShouldHavePermission(SchoolAction.Create, SchoolFeature.Tenants)]
    public async Task<IActionResult> ActiveTenantAsync([FromQuery] string tenantId)
    {
        var response = await Sender.Send(new ActiveTenantCommand
        {
            TenantId = tenantId
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpPost("{tenantId}/deactivate")]
    [ShouldHavePermission(SchoolAction.Create, SchoolFeature.Tenants)]
    public async Task<IActionResult> DeactiveTenantAsync([FromQuery] string tenantId)
    {
        var response = await Sender.Send(new DeactiveTenantCommand
        {
            TenantId = tenantId
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpPut("upgrade")]
    [ShouldHavePermission(SchoolAction.UpgradeSubscription, SchoolFeature.Tenants)]
    public async Task<IActionResult> UpgradeTenantSubscriptionAsync([FromBody] UpdateTenantSubscriptionRequest updateTenant)
    {
        var response = await Sender.Send(new UpdateTenantSubscriptionCommand
        {
            UpdateTenantSubscription = updateTenant
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpGet("{tenantId")]
    [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Tenants)]
    public async Task<IActionResult> GetTenantByIdAsync([FromQuery] string tenantId)
    {
        var response = await Sender.Send(new GetTenantByIdQuery
        {
            TenantId = tenantId
        });

        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpGet("all")]
    [ShouldHavePermission(SchoolAction.Read, SchoolFeature.Tenants)]
    public async Task<IActionResult> GetTenantsAsync()
    {
        var response = await Sender.Send(new GetTenantsQuery());
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
}