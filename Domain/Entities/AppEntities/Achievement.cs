using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Domain.Entities.AppEntities
{

    public class Achievement : BaseEntity
    {

        public int AchievementId { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Criteria {  get; set; } = string.Empty;

        public ICollection<UserAchievement> UserAchievements { get; set; } = [];
    }
}
