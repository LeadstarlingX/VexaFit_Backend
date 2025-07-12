﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IAppServices.Authentication
{
    public interface IAuthenticationService
    {
        Task<long> RegisterAsync(RegisterDto dto);
        Task<UserProfileDto> LoginAsync(LoginDto loginDto);
        Task<UserProfileDto> GetAuthenticatedUser();
        Task LogoutAsync();
    }
}
