using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser,MemberDto>()
            .ForMember(x=>x.PhotoUrl, opt=>opt.MapFrom(y=>y.Photos.FirstOrDefault(z=>z.isMain).Url))
            .ForMember(x=>x.Age, opt=> opt.MapFrom(y=>y.DateOfBirth.CalculateAge()));  
            CreateMap<Photo, PhotoDto>();        
            CreateMap<UpdateMemberDto,AppUser>();
            CreateMap<RegisterDto, AppUser>();
            CreateMap<Message,MessageDto>()
            .ForMember(x=>x.SenderImageUrl, opt=>opt.MapFrom(y=>y.Sender.Photos.FirstOrDefault(z=>z.isMain).Url))
            .ForMember(x=>x.RecipientImageUrl, opt=>opt.MapFrom(y=>y.Recipient.Photos.FirstOrDefault(z=>z.isMain).Url));
            CreateMap<DateTime,DateTime>().ConvertUsing(d=>DateTime.SpecifyKind(d,DateTimeKind.Local));
            CreateMap<MessageDto, Message>();
        }
    }
}