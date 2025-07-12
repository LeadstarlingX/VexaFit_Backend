using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Achievement;

namespace Application.IAppServices.Common
{
    public interface IService<TEntityDTO, TCreateDTO, TUpdateDTO>
    {
        Task<TEntityDTO> GetByIdAsync(int id);
        Task<IEnumerable<TEntityDTO>> GetAllAsync();
        Task<TEntityDTO> CreateAsync(TCreateDTO dto);
        Task<IEnumerable<TEntityDTO>> CreateBulkAsync(IEnumerable<TCreateDTO> dtos);
        Task<TEntityDTO> UpdateAsync(TUpdateDTO dto);
        Task<IEnumerable<TEntityDTO>> UpdateBulkAsync(IEnumerable<TUpdateDTO> dto);
        Task DeleteAsync(int id);
        Task DeleteBulkAsync(IEnumerable<int> ids);
    }
}
