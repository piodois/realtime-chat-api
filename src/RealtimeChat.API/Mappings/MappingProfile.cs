using AutoMapper;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Shared.DTOs;

namespace RealtimeChat.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<ChatRoom, ChatRoomDto>()
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.ChatRoomUsers.Count));
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
    }
}