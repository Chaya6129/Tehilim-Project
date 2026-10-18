using AutoMapper;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Entities;

namespace Tehilim.Service.Profiles
{
    public class ChapterProfile : Profile
    {
        public ChapterProfile()
        {
            CreateMap<Chapter, ChapterDto>()
                .ForCtorParam(nameof(ChapterDto.Status), opt => opt.MapFrom(src => src.Status.ToString()))
                // שם המשתמש הרשום אם יש, אחרת שם האורח — מאוחד לשדה אחד
                .ForCtorParam(nameof(ChapterDto.TakenByName), opt => opt.MapFrom(src =>
                    src.TakenByUser != null ? src.TakenByUser.DisplayName : src.TakenByName));
        }
    }
}