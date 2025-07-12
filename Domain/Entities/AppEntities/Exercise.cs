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
        public int ExerciseId { get; set; }
        public int ExerciseCategoryId { get; set; }
        public int PositionCategoryId { get; set; }
        public int ImageId { get; set; }
        public int VideoId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = []!;
        public ICollection<ExercisePosition>? ExercisePositions { get; set; } = [];
        public Image Image { get; set; } = null!;
        public Video Video { get; set; } = null!;
        public Category Category { get; set; } = null!;


    }
}
