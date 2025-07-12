using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Authentication;

namespace Application.IAppServices.Authentication
{
    public interface IAuthenticationService
    {
        Task<long> RegisterAsync(RegisterDTO dto);
        Task<UserProfileDTO> LoginAsync(LoginDTO dto);
        Task<UserProfileDTO> GetAuthenticatedUser();
        Task LogoutAsync();
    }
}
