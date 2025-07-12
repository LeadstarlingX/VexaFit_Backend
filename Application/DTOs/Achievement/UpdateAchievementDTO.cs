using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Common;

namespace Application.DTOs.Achievement
{
    public class UpdateAchievementDTO : BaseDTO<int>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Criteria { get; set; }
    }
}
