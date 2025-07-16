using Application.Common;
using Application.DTOs.Action;
using Application.DTOs.Authentication;
using Application.IAppServices.Authentication;
using Application.Serializer;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IJsonFieldsSerializer _jsonFieldsSerializer;

        public AuthController(IAuthenticationService authService, IJsonFieldsSerializer jsonFieldsSerializer)
        {
            _authService = authService;
            _jsonFieldsSerializer = jsonFieldsSerializer;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserProfileDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            return new RawJsonActionResult(_jsonFieldsSerializer.Serialize(new ApiResponse(true, "", StatusCodes.Status200OK, response), string.Empty));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            return new RawJsonActionResult(_jsonFieldsSerializer.Serialize(new ApiResponse(true, "User registered successfully", StatusCodes.Status201Created, result), string.Empty));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return new RawJsonActionResult(_jsonFieldsSerializer.Serialize(new ApiResponse(true, "Logout successfully", StatusCodes.Status200OK), string.Empty));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<UserProfileDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuthenticated()
        {
            var user = await _authService.GetAuthenticatedUser();
            return new RawJsonActionResult(_jsonFieldsSerializer.Serialize(new ApiResponse(true, "", StatusCodes.Status200OK, user), string.Empty));
        }
    }
}
