using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    private readonly IUserRepository userRepository;

    public AutoMapperProfiles(IUserRepository userRepository) 
    {
        this.userRepository = userRepository;
    }

    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>();
        CreateMap<Language, LanguageDto>();
        CreateMap<Settings, SettingsDto>();
        CreateMap<Statistics, StatisticsDto>();
    }
}
