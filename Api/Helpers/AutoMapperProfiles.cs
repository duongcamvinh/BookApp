using api.DTO;
using Api.DTO;
using Api.Entities;
using AutoMapper;
using CloudinaryDotNet;
using System.Linq;

namespace Api.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<RegisterDTO, AppUsers>();
            CreateMap<AppBooks, BooksDto>();
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUsers>();
            CreateMap<BookUpdateDto, AppBooks>();
            CreateMap<AppUsers, MemberDto>();
            
        }
    }
}
