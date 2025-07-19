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
            CreateMap<Exercise, ExerciseDTO>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src =>
                    src.ExerciseCategories.Select(ec => ec.Category)));

            CreateMap<Image, ImageDTO>();
            CreateMap<Video, VideoDTO>();
            CreateMap<Category, CategoryDTO>();

            CreateMap<CreateExerciseDTO, ExerciseCategory>();
        }
    }
}
