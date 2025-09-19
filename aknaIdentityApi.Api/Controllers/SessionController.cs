// SessionController.cs
using aknaIdentity_api.Domain.Dtos;
using aknaIdentity_api.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aknaIdentity_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IAuditService _auditService;
        private readonly ILogger<SessionController> _logger;

        public SessionController(
            ISessionService sessionService,
            IAuditService auditService,
            ILogger<SessionController> logger)
        {
            _sessionService = sessionService;
            _auditService = auditService;
            _logger = logger;
        }

        [HttpGet("my-sessions")]
        public async Task<IActionResult> GetMySessions()
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var sessions = await _sessionService.GetUserSessionsAsync(userId);

                var response = sessions.Select(s => new SessionDto
                {
                    Id = s.Id,
                    LoginTime = s.LoginTime,
                    LogoutTime = s.LogoutTime,
                    ExpirationTime = s.ExpirationTime,
                    DeviceInfo = s.DeviceInfo,
                    IpAddress = s.IpAddress,
                    IsActive = !s.LogoutTime.HasValue && (!s.ExpirationTime.HasValue || s.ExpirationTime > DateTime.UtcNow)
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user sessions");
                return StatusCode(500, new { message = "An error occurred while fetching sessions" });
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveSessions()
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var sessions = await _sessionService.GetActiveSessionsAsync(userId);

                var response = sessions.Select(s => new SessionDto
                {
                    Id = s.Id,
                    LoginTime = s.LoginTime,
                    LogoutTime = s.LogoutTime,
                    ExpirationTime = s.ExpirationTime,
                    DeviceInfo = s.DeviceInfo,
                    IpAddress = s.IpAddress,
                    IsActive = true
                }).ToList();

                return Ok(new
                {
                    sessions = response,
                    activeCount = response.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active sessions");
                return StatusCode(500, new { message = "An error occurred while fetching sessions" });
            }
        }

        [HttpPost("{sessionId}/terminate")]
        public async Task<IActionResult> TerminateSession(int sessionId)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                // Verify session belongs to user
                var sessions = await _sessionService.GetUserSessionsAsync(userId);
                if (!sessions.Any(s => s.Id == sessionId))
                    return Forbid();

                await _sessionService.TerminateSessionAsync(sessionId);
                await _auditService.LogUserActionAsync(userId, "SessionTerminate", $"Terminated session {sessionId}");

                return Ok(new { message = "Session terminated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error terminating session: {SessionId}", sessionId);
                return StatusCode(500, new { message = "An error occurred while terminating session" });
            }
        }

        [HttpPost("terminate-all")]
        public async Task<IActionResult> TerminateAllSessions()
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                await _sessionService.TerminateUserSessionsAsync(userId);
                await _auditService.LogUserActionAsync(userId, "SessionTerminateAll", "Terminated all sessions");

                return Ok(new { message = "All sessions terminated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error terminating all sessions");
                return StatusCode(500, new { message = "An error occurred while terminating sessions" });
            }
        }

        [HttpPost("terminate-others")]
        public async Task<IActionResult> TerminateOtherSessions()
        {
            try
            {
                var userIdClaim = User.FindFirst("sub")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                // Get current session ID from token or headers
                var sessionIdClaim = User.FindFirst("session_id")?.Value;
                if (!int.TryParse(sessionIdClaim, out int currentSessionId))
                    return BadRequest(new { message = "Current session ID not found" });

                await _sessionService.TerminateUserSessionsExceptCurrentAsync(userId, currentSessionId);
                await _auditService.LogUserActionAsync(userId, "SessionTerminateOthers", "Terminated all other sessions");

                return Ok(new { message = "Other sessions terminated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error terminating other sessions");
                return StatusCode(500, new { message = "An error occurred while terminating sessions" });
            }
        }
    }
}