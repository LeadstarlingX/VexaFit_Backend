using Application.DTOs.Authentication;
using Application.IAppServices.Authentication;
using Application.Serializer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Common
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    public class BaseAuthenticatedController(IAuthenticationService authenticationService, IJsonFieldsSerializer jsonFieldsSerializer) : ControllerBase
    {
        protected readonly IAuthenticationService _authenticationService = authenticationService;
        protected readonly IJsonFieldsSerializer _jsonFieldsSerializer = jsonFieldsSerializer;

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<UserProfileDTO> GetCurrentUserAsync()
        {
            return await _authenticationService.GetAuthenticatedUser();
        }
    }
}
