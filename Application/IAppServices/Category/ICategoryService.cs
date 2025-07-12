using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Achievement;

namespace Application.IAppServices.Category
{
    public interface ICategoryService
    {
        Task<AchievementDTO> GetByIdAsync(int id);
        Task<IEnumerable<AchievementDTO>> GetAllAsync();
        Task<AchievementDTO> CreateAsync(CreateAchievementDTO dto);
        Task<IEnumerable<AchievementDTO>> CreateBulkAsync(IEnumerable<CreateAchievementDTO> dtos);
        Task<AchievementDTO> UpdateAsync(UpdateAchievementDTO dto);
        Task<IEnumerable<AchievementDTO>> UpdateBulkAsync(IEnumerable<UpdateAchievementDTO> dto);
        Task DeleteAsync(int id);
        Task DeleteBulkAsync(IEnumerable<int> ids);
    }
}
