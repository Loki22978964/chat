using Application.DTOs;
using AutoMapper;
using Domain.Entities;


namespace Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domain.Entities.Chat, ChatDto>()
                .ForMember(chatDto => chatDto.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(chatDto => chatDto.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<Message, MessageDto>()
                .ForMember(messDto => messDto.Id , src => src.MapFrom(src => src.Id))
                .ForMember(messDto => messDto.UserName, src => src.MapFrom(src => src.User.Name))
                .ForMember(messDto => messDto.Content, src => src.MapFrom(src => src.Content))
                .ForMember(messDto => messDto.Timestamp, src => src.MapFrom(src => src.Timestamp))
                .ForMember(messDto => messDto.Status, src => src.MapFrom(src => src.Status))
                .ForMember(messDto => messDto.UserId, src => src.MapFrom(src => src.UserId))
                .ForMember(messDto => messDto.ChatId, src => src.MapFrom(src => src.ChatId));
        }
    }
}
