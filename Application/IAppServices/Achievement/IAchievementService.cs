using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Achievement;
using Application.IAppServices.Common;

namespace Application.IAppServices.Achievement
{
    public interface IAchievementService : IService<AchievementDTO, CreateAchievementDTO, UpdateAchievementDTO>
    {

    }
}
