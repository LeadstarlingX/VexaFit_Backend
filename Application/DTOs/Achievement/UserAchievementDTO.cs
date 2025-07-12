using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;

namespace Application.DTOs.Achievement
{
    public class UserAchievementDTO : BaseDTO<int>
    {
        public int UserId { get; set; }
        public int AchievementId { get; set; }
        public DateTime DateUnlocked { get; set; }

    }
}
