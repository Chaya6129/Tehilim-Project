using AutoMapper;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.Entities;

namespace Tehilim.Service.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // הרשמה → ישות. הסיסמה לא מועתקת: ה-hash, ה-Role ותאריך היצירה נקבעים ב-AuthService (שלב 6)
            CreateMap<RegisterRequest, User>(MemberList.Source)
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FullName))
                .ForSourceMember(src => src.Password, opt => opt.DoNotValidate());
        }
    }
}