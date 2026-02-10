using AutoMapper;
using fitness_dash_api.Objects;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace fitness_dash_api.Mapping
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<User, UserResponse>();
            CreateMap<Exercise, ExerciseCreate>();
        }
    }
}
