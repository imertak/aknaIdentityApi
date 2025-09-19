using aknaIdentity_api.Domain.Dtos;
using aknaIdentity_api.Domain.Interfaces.Services;
using aknaIdentity_api.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aknaIdentity_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IAuditService _auditService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            IRoleService roleService,
            IAuditService auditService,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _auditService = auditService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string searchTerm = null)
        {
            try
            {
                var result = await _userService.GetUsersPagedAsync(page, pageSize, searchTerm);

                var response = new PagedResponse<UserDto>
                {
                    Data = result.Users.Select(u => new UserDto
                    {
                        Id = u.Id,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        PhoneNumber = u.PhoneNumber,
                        UserType = u.UserType,
                        AccountStatus = u.AccountStatus,
                        IsEmailConfirmed = u.IsEmailConfirmed,
                        IsPhoneNumberConfirmed = u.IsPhoneNumberConfirmed,
                        Roles = u.Roles?.Select(r => new RoleDto
                        {
                            Id = r.Id,
                            Name = r.Name,
                            Description = r.Description
                        }).ToList()
                    }).ToList(),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = result.TotalCount,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return StatusCode(500, new { message = "An error occurred while fetching users" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int currentUserId))
                    return Unauthorized();

                // Users can view their own profile, admins can view any profile
                if (currentUserId != id && !User.IsInRole(Roles.Admin))
                    return Forbid();

                var user = await _userService.GetUserWithRolesAndPermissionsAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                var userDto = new UserDetailDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    UserType = user.UserType,
                    AccountStatus = user.AccountStatus,
                    IsEmailConfirmed = user.IsEmailConfirmed,
                    IsPhoneNumberConfirmed = user.IsPhoneNumberConfirmed,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    Roles = user.Roles?.Select(r => new RoleDto
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
                    }).ToList()
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by id: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while fetching user" });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                return await GetUserById(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, new { message = "An error occurred while fetching user profile" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int currentUserId))
                    return Unauthorized();

                // Users can update their own profile, admins can update any profile
                if (currentUserId != id && !User.IsInRole(Roles.Admin))
                    return Forbid();

                var user = await _userService.UpdateUserAsync(id, request.FirstName, request.LastName, request.PhoneNumber);

                if (user == null)
                    return NotFound(new { message = "User not found" });

                await _auditService.LogUserActionAsync(currentUserId, "UserUpdate", $"Updated user {id}");

                return Ok(new { message = "User updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating user" });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] UpdateUserStatusRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int currentUserId))
                    return Unauthorized();

                var user = await _userService.UpdateUserStatusAsync(id, request.Status);

                if (user == null)
                    return NotFound(new { message = "User not found" });

                await _auditService.LogUserActionAsync(currentUserId, "UserStatusUpdate", $"Updated user {id} status to {request.Status}");

                return Ok(new { message = "User status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user status: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating user status" });
            }
        }

        [HttpPost("{id}/roles")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> AssignRolesToUser(int id, [FromBody] AssignRolesRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int currentUserId))
                    return Unauthorized();

                await _roleService.AssignRolesToUserAsync(id, request.RoleIds);

                await _auditService.LogUserActionAsync(currentUserId, "RoleAssignment", $"Assigned roles to user {id}");

                return Ok(new { message = "Roles assigned successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning roles to user: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while assigning roles" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int currentUserId))
                    return Unauthorized();

                if (currentUserId == id)
                    return BadRequest(new { message = "You cannot delete your own account" });

                await _userService.DeleteUserAsync(id, currentUserId);

                await _auditService.LogUserActionAsync(currentUserId, "UserDelete", $"Deleted user {id}");

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting user" });
            }
        }

        [HttpPost("{id}/restore")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> RestoreUser(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int currentUserId))
                    return Unauthorized();

                var user = await _userService.RestoreUserAsync(id);

                if (user == null)
                    return NotFound(new { message = "User not found" });

                await _auditService.LogUserActionAsync(currentUserId, "UserRestore", $"Restored user {id}");

                return Ok(new { message = "User restored successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring user: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while restoring user" });
            }
        }

        [HttpGet("by-role/{roleId}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> GetUsersByRole(int roleId)
        {
            try
            {
                var users = await _userService.GetUsersByRoleAsync(roleId);

                var response = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    UserType = u.UserType,
                    AccountStatus = u.AccountStatus,
                    IsEmailConfirmed = u.IsEmailConfirmed,
                    IsPhoneNumberConfirmed = u.IsPhoneNumberConfirmed
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by role: {RoleId}", roleId);
                return StatusCode(500, new { message = "An error occurred while fetching users" });
            }
        }

        [HttpGet("by-status/{status}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> GetUsersByStatus(string status)
        {
            try
            {
                var users = await _userService.GetUsersByStatusAsync(status);

                var response = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    UserType = u.UserType,
                    AccountStatus = u.AccountStatus,
                    IsEmailConfirmed = u.IsEmailConfirmed,
                    IsPhoneNumberConfirmed = u.IsPhoneNumberConfirmed
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by status: {Status}", status);
                return StatusCode(500, new { message = "An error occurred while fetching users" });
            }
        }
    }
}