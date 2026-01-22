# Copilot Instructions

## General Guidelines
- Prefer using Mapster for DTO-to-entity mapping (use `request.Adapt(existingEntity)` when updating entities).
- When updating role permissions, include permission metadata (Description and Group) in `UpdateRolePermissionsRequest.NewPermissions` instead of hardcoding empty strings in `RoleService`; prefer the request to carry Name, Description, and Group for each permission.