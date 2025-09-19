// RoleController.cs
using aknaIdentity_api.Domain.Dtos;
using aknaIdentity_api.Domain.Interfaces.Services;
using aknaIdentity_api.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Octokit;

namespace aknaIdentity_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireAdminRole")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IAuditService _auditService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(
            IRoleService roleService,
            IAuditService auditService,
            ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _auditService = auditService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetRolesWithPermissionsAsync();

                var response = roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Permissions = r.Permissions?.Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description
                    }).ToList()
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles");
                return StatusCode(500, new { message = "An error occurred while fetching roles" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            try
            {
                var role = await _roleService.GetRoleWithPermissionsAsync(id);

                if (role == null)
                    return NotFound(new { message = "Role not found" });

                var response = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    Permissions = role.Permissions?.Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while fetching role" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var role = await _roleService.CreateRoleAsync(request.Name, request.Description);

                await _auditService.LogUserActionAsync(userId, "RoleCreate", $"Created role: {request.Name}");

                return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, new { id = role.Id, message = "Role created successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return StatusCode(500, new { message = "An error occurred while creating role" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var role = await _roleService.UpdateRoleAsync(id, request.Name, request.Description);

                if (role == null)
                    return NotFound(new { message = "Role not found" });

                await _auditService.LogUserActionAsync(userId, "RoleUpdate", $"Updated role {id}");

                return Ok(new { message = "Role updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating role" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                await _roleService.DeleteRoleAsync(id, userId);

                await _auditService.LogUserActionAsync(userId, "RoleDelete", $"Deleted role {id}");

                return Ok(new { message = "Role deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting role" });
            }
        }

        [HttpPost("{id}/permissions")]
        public async Task<IActionResult> AssignPermissionsToRole(int id, [FromBody] AssignPermissionsRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                await _roleService.AssignPermissionsToRoleAsync(id, request.PermissionIds);

                await _auditService.LogUserActionAsync(userId, "PermissionAssignment", $"Assigned permissions to role {id}");

                return Ok(new { message = "Permissions assigned successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permissions to role: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while assigning permissions" });
            }
        }
    }
}