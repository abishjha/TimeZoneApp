using AutoMapper;
using TimeZoneWebApi.Entities;
using TimeZoneWebApi.Models.Users;
using TimeZoneWebApi.Models.TimeZones;

namespace TimeZoneWebApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<Models.Users.UserRegisterModel, User>();
            CreateMap<Models.Users.UserUpdateModel, User>();

            CreateMap<TimeZone, TimeZoneModel>();
            CreateMap<Models.TimeZones.TimeZoneCreateModel, TimeZone>();
            CreateMap<Models.TimeZones.TimeZoneUpdateModel, TimeZone>();
        }
    }
}