using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Domain.Entities.Email;
using BulkMailSender.Domain.ValueObjects;
using Attachment = BulkMailSender.Domain.ValueObjects.Attachment;
using InlineResource = BulkMailSender.Domain.ValueObjects.InlineResource;

namespace BulkMailSender.Application.Mappings {
    public class ApplicationMappingProfile : Profile {
        public ApplicationMappingProfile() {
            CreateMap<Requester, RequesterDto>()
                .ForMember(dest => dest.Server, opt => opt.MapFrom(src => src.Server)); // Map nested MailServerDto
            CreateMap<RequesterDto, Requester>()
                .ForMember(dest => dest.Server, opt => opt.MapFrom(src => src.Server));

            CreateMap<MailServer, MailServerDto>().ReverseMap();

            CreateMap<EmailDto, Email>()
                   .ForMember(dest => dest.EmailFrom, opt => opt.MapFrom(src => new EmailAddress(src.EmailFrom)))
                   .ForMember(dest => dest.EmailTo, opt => opt.MapFrom(src => new EmailAddress(src.EmailTo)))
                   .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId)) // Map StatusId directly
                   .ForMember(dest => dest.Status, opt => opt.Ignore()) // Ignore the derived Status property
                                                                        //.ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments.Select(CreateAttachment)))
                                                                        //.ForMember(dest => dest.InlineResources, opt => opt.MapFrom(src => src.InlineResources.Select(CreateInlineResource)));
             .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments.Select(dto => new Attachment(dto.FileName, dto.Content))))
    .ForMember(dest => dest.InlineResources, opt => opt.MapFrom(src => src.InlineResources.Select(dto => new InlineResource(dto.FileName, dto.Content))));

            CreateMap<Email, EmailDto>()
                .ForMember(dest => dest.EmailFrom, opt => opt.MapFrom(src => src.EmailFrom.Value))
                .ForMember(dest => dest.EmailTo, opt => opt.MapFrom(src => src.EmailTo.Value))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
                 .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments))
                .ForMember(dest => dest.InlineResources, opt => opt.MapFrom(src => src.InlineResources));


        }

        private static Domain.ValueObjects.Attachment CreateAttachment(AttachmentDto dto) {
            return new Domain.ValueObjects.Attachment(dto.FileName, dto.Content);
        }
        private static Domain.ValueObjects.InlineResource CreateInlineResource(InlineResourceDto dto) {
            return new Domain.ValueObjects.InlineResource(dto.FileName, dto.Content);
        }
    }
}
