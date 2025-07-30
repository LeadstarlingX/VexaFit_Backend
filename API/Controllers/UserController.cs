using API.Controllers.Common;
using Application.Common;
using Application.DTOs.Action;
using Application.IAppServices.Authentication;
using Application.IAppServices.User;
using Application.Serializer;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = DefaultSettings.AdminRoleName)]
    public class UserController : BaseAuthenticatedController
    {
        private readonly IUserService _userService;

        public UserController(
            IUserService userService,
            IAuthenticationService authenticationService,
            IJsonFieldsSerializer jsonFieldsSerializer) : base(authenticationService, jsonFieldsSerializer)
        {
            _userService = userService;
        }

        [HttpGet("stats")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _userService.GetDashboardStatsAsync();
            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "Dashboard statistics retrieved successfully.", StatusCodes.Status200OK, stats),
                    string.Empty));
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return new RawJsonActionResult(
                _jsonFieldsSerializer.Serialize(
                    new ApiResponse(true, "All users retrieved successfully.", StatusCodes.Status200OK, users),
                    string.Empty));
        }
    }
}
