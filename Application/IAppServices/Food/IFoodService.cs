using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Food;
using Application.IAppServices.Common;

namespace Application.IAppServices.Food
{
    internal interface IFoodService : IService<FoodDTO, CreateFoodDTO, UpdateFoodDTO>
    {
    }
}
