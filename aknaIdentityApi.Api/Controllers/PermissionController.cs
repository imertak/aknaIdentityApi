// PermissionController.cs
using aknaIdentity_api.Domain.Dtos;
using aknaIdentity_api.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aknaIdentity_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireAdminRole")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly IAuditService _auditService;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(
            IPermissionService permissionService,
            IAuditService auditService,
            ILogger<PermissionController> logger)
        {
            _permissionService = permissionService;
            _auditService = auditService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPermissions()
        {
            try
            {
                var permissions = await _permissionService.GetAllPermissionsAsync();

                var response = permissions.Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions");
                return StatusCode(500, new { message = "An error occurred while fetching permissions" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            try
            {
                var permission = await _permissionService.GetByIdAsync(id);

                if (permission == null)
                    return NotFound(new { message = "Permission not found" });

                var response = new PermissionDto
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Description = permission.Description
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permission: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while fetching permission" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var permission = await _permissionService.CreatePermissionAsync(request.Name, request.Description);

                await _auditService.LogUserActionAsync(userId, "PermissionCreate", $"Created permission: {request.Name}");

                return CreatedAtAction(nameof(GetPermissionById), new { id = permission.Id }, new { id = permission.Id, message = "Permission created successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission");
                return StatusCode(500, new { message = "An error occurred while creating permission" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] UpdatePermissionRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var permission = await _permissionService.UpdatePermissionAsync(id, request.Name, request.Description);

                if (permission == null)
                    return NotFound(new { message = "Permission not found" });

                await _auditService.LogUserActionAsync(userId, "PermissionUpdate", $"Updated permission {id}");

                return Ok(new { message = "Permission updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permission: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating permission" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                await _permissionService.DeletePermissionAsync(id, userId);

                await _auditService.LogUserActionAsync(userId, "PermissionDelete", $"Deleted permission {id}");

                return Ok(new { message = "Permission deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting permission: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting permission" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPermissions(int userId)
        {
            try
            {
                var permissions = await _permissionService.GetUserPermissionsAsync(userId);

                var response = permissions.Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user permissions: {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while fetching permissions" });
            }
        }

        [HttpPost("check")]
        [Authorize]
        public async Task<IActionResult> CheckUserPermission([FromBody] CheckPermissionRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var hasPermission = await _permissionService.UserHasPermissionAsync(userId, request.PermissionName);

                return Ok(new { hasPermission });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission");
                return StatusCode(500, new { message = "An error occurred while checking permission" });
            }
        }
    }
}