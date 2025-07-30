using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.User;

namespace Application.IAppServices.User
{
    public interface IUserService
    {
        Task<AdminDashboardStatsDTO> GetDashboardStatsAsync();
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task ToggleUserStatusAsync(string userId);
    }
}
