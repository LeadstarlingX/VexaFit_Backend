using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Category;
using Application.DTOs.Exercise;
using Application.IAppServices.Common;
namespace Application.IAppServices.Exercise
{
    public interface IExerciseService : IService<ExerciseDTO, CreateExerciseDTO, UpdateExerciseDTO>
    {

    }
}
