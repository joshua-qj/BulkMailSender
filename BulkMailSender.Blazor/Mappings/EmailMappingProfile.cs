using AutoMapper;

namespace BulkMailSender.Blazor.Mappings {
    public class EmailMappingProfile : Profile {
        public EmailMappingProfile() {
            //CreateMap<EmailViewModel, EmailDto>()
            //    .ForMember(dest => dest.EmailFrom, opt => opt.MapFrom(src => src.EmailFrom.ToLower())) // Example transformation
            //    .ReverseMap(); // Map back if needed
        }
    }
}
