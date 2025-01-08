using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Blazor.ViewModels;
using BulkMailSender.Blazor.ViewModels.UserViewModels;
using BulkMailSender.Domain.Entities.Identity;
using BulkMailSender.Infrastructure.Common.Entities.Identity;

namespace BulkMailSender.Blazor.Mappings {
    public class BlazorMappingProfile : Profile {
        public BlazorMappingProfile() {
            CreateMap<EmailViewModel, EmailDto>()
             .ForMember(dest => dest.IsBodyHtml, opt => opt.MapFrom(src => true))
             .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => 1))
             .ForMember(dest => dest.RequestedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
             .ForMember(dest => dest.Requester, opt => opt.Ignore());

            CreateMap<AttachmentViewModel, AttachmentDto>();

            CreateMap<InlineResourceViewModel, InlineResourceDto>();

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


            CreateMap<UserViewModel, UserDto>()
   .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Convert Guid to string
   .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
   .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
   // .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
   .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            // Map from ApplicationUser to Domain User
            CreateMap<UserDto, UserViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src =>src.Id)) // Convert string to Guid
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
             //   .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

        }
    }
}
