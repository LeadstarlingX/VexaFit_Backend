using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.AppEntities
{
    public class CustomWorkout : Workout
    {
        public string UserId { get; set; } = string.Empty;


        public DateTime CreationDate { get; set; }
    }
}
