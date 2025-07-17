using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Domain.Entities.AppEntities
{
    public class Exercise : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;


        public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; } = [];
        public virtual ICollection<Image> Images { get; set; } = [];
        public virtual ICollection<Video> Videos { get; set; } = [];
        public virtual ICollection<Category> Categories { get; set; } = [];


    }
}
