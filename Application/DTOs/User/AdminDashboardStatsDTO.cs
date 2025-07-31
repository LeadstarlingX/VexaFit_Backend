using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User
{
    public class AdminDashboardStatsDTO
    {
        public int TotalUsers { get; set; }
        public int TotalWorkouts { get; set; }
        public int TotalExercises { get; set; }
        public int ActiveUsersToday { get; set; }
    }
}
