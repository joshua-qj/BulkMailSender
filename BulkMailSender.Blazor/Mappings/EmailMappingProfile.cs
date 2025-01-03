using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Blazor.ViewModels;

namespace BulkMailSender.Blazor.Mappings {
    public class EmailMappingProfile : Profile {
        public EmailMappingProfile() {

            CreateMap<(ExcelImportDto, EmailViewModel), EmailDto>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => $"{src.Item1.FirstName} {src.Item1.LastName}"))
            .ForMember(dest => dest.EmailTo, opt => opt.MapFrom(src => src.Item1.EmailAddress))
            .ForMember(dest => dest.EmailFrom, opt => opt.MapFrom(src => src.Item2.EmailFrom))
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Item2.Subject))
            .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Item2.Body))
            .ForMember(dest => dest.IsBodyHtml, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => 1))
            .ForMember(dest => dest.RequestedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Item2.Attachments
                .Select(a => new AttachmentDto {
                    Id = a.Id,
                    FileName = a.FileName,
                    Content = a.Content
                }).ToList()))
            .ForMember(dest => dest.InlineResources, opt => opt.MapFrom(src => src.Item2.InlineResources
                .Select(r => new InlineResourceDto {
                    Id = r.Id,
                    FileName = r.FileName,
                    Content = r.Content,
                    MimeType = r.MimeType
                }).ToList()));

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
