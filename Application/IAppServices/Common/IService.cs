using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IAppServices.Common
{
    public interface IService<TEntityDTO, TCreateDTO, TUpdateDTO, TGetDTO>
    {
        Task<TEntityDTO> GetByIdAsync(int id);
        Task<IEnumerable<TEntityDTO>> GetAllAsync(TGetDTO dto);
        Task<TEntityDTO> CreateAsync(TCreateDTO dto);
        Task<IEnumerable<TEntityDTO>> CreateBulkAsync(IEnumerable<TCreateDTO> dtos);
        Task<TEntityDTO> UpdateAsync(TUpdateDTO dto);
        Task<IEnumerable<TEntityDTO>> UpdateBulkAsync(IEnumerable<TUpdateDTO> dto);
        Task DeleteAsync(int id);
        Task DeleteBulkAsync(IEnumerable<int> ids);
    }
}
