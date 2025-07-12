using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.AppEntities
{
    public class UserAchievement : BaseEntity
    {
        public Guid UserId { get; set; }
        public int AchievementId { get; set; }
        public DateTime DateUnlocked { get; set; }
        public Achievement Achievement { get; set; } = null!;
    }
}
