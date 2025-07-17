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
        public string Name { get; set; } = string.Empty;
        public CategoryTypeEnum Type { get; set; }
        

        public virtual ICollection<ExerciseCategory> ExerciseCategories { get; set; } = [];
    }
}
