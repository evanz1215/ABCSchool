using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Auth;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; set; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}