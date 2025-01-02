using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Blazor.ViewModels;

namespace BulkMailSender.Blazor.Mappings {
    public class EmailMappingProfile : Profile {
        public EmailMappingProfile() {
            // Map EmailViewModel to EmailDto
            CreateMap<EmailViewModel, EmailDto>()
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments))
                .ForMember(dest => dest.InlineResources, opt => opt.MapFrom(src => src.InlineResources));

            // Optionally, map back from EmailDto to EmailViewModel
            CreateMap<EmailDto, EmailViewModel>();
            CreateMap<AttachmentViewModel, AttachmentDto>();

            // Map RequesterViewModel to RequesterDto
            CreateMap<RequesterViewModel, RequesterDto>()
                .ForMember(dest => dest.Server, opt => opt.MapFrom(src => src.Server)); // Map nested MailServerDto

            // Map RequesterDto to RequesterViewModel
            CreateMap<RequesterDto, RequesterViewModel>()
                .ForMember(dest => dest.Server, opt => opt.MapFrom(src => src.Server)); // Map nested MailServerDto

            // Map MailServerDto to MailServerViewModel
            CreateMap<MailServerDto, MailServerViewModel>().ReverseMap();
        }
    }
}
