using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.AppEntities
{
    public class Video : BaseEntity
    {
        public int VideoId { get; set; }
        public int ExerciseId { get; set; }


        public string Url { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public virtual Exercise Exercise { get; set; } = null!;
    }
}
