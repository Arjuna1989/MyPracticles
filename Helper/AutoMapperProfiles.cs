using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using API.Dto;
using API.Entity;
using API.Extensions;
using AutoMapper;

namespace API.Helper
{
    public class AutoMapperProfiles :Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,UserDto>()
             .ForMember(dest => dest.PhotoUrl , opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p =>p.IsMain).Url))
             .ForMember(dest => dest.Age , opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<Photo,PhotoDto>();
            CreateMap<UserUpdateDto,User>();
        }
        
    }
}