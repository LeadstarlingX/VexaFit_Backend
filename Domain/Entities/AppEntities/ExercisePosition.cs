using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.AppEntities
{
    public class ExercisePosition : BaseEntity
    {
        public int ExerciseId { get; set; }
        public int CategoryId { get; set; }

        public Exercise Exercise { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}
