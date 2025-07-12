using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;
using Domain.Enum;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Domain.Entities.AppEntities
{
    public class Category : BaseEntity
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public CategoryTypeEnum CategoryType { get; set; }

        public ICollection<Exercise> Exercises { get; set; } = []!;
        public ICollection<ExercisePosition>? ExercisePositions { get; set; } = [];
    }
}
