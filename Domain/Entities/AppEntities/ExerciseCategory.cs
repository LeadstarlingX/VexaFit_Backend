using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.AppEntities
{
    public  class ExerciseCategory : BaseEntity
    {
        public int CategoryId { get; set; }
        public int ExerciseId { get; set; }


        public virtual Category Category { get; set; } = null!;
        public virtual Exercise Exercise { get; set; } = null!;
    }
}
