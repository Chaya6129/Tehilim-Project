using AutoMapper;
using Tehilim.Core.DTOs.Requests;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Entities;

namespace Tehilim.Service.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            // ישות → תגובה. DTO הוא record, לכן ההגדרה היא על פרמטרי ה-constructor
            CreateMap<Book, BookDetailsDto>()
                .ForCtorParam(nameof(BookDetailsDto.Goal), opt => opt.MapFrom(src => src.Purpose))
                .ForCtorParam(nameof(BookDetailsDto.Status), opt => opt.MapFrom(src => src.Status.ToString()))
                .ForCtorParam(nameof(BookDetailsDto.OwnerName), opt => opt.MapFrom(src => src.Owner.DisplayName));

            // בקשה → ישות חדשה. MemberList.Source: בודקים רק ששדות הבקשה מופו,
            // שאר שדות הישות (בעלים, סטטוס, תאריכים, פרקים) נקבעים ב-Service
            CreateMap<CreateBookRequest, Book>(MemberList.Source)
                .ForMember(dest => dest.Purpose, opt => opt.MapFrom(src => src.Goal));

            // בקשה → ישות קיימת. משנה יעד ופומביות בלבד; שום שדה אחר לא נדרס
            CreateMap<UpdateBookRequest, Book>(MemberList.Source)
                .ForMember(dest => dest.Purpose, opt => opt.MapFrom(src => src.Goal));
        }
    }
}