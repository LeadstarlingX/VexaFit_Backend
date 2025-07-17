using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Category;
using Application.DTOs.Exercise;
using AutoMapper;
using Domain.Entities.AppEntities;

namespace Application.Mapping.ExerciseProfile
{
    public class ExerciseProfile : Profile
    {
        public ExerciseProfile()
        {
            CreateMap<CreateExerciseDTO, Exercise>();
            CreateMap<UpdateExerciseDTO, Exercise>();
            CreateMap<Exercise, ExerciseDTO>();

            CreateMap<CreateExerciseDTO, ExerciseCategory>();
        }
    }
}
