using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.AppEntities
{
    public class CustomWorkout : Workout
    {
        public int CustomWorkoutId { get; set; }
        public int UserId { get; set; }


        public DateTime CreationDate { get; set; }
    }
}
