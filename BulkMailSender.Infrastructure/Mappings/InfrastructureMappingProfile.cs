﻿using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Domain.Entities.Email;
using BulkMailSender.Domain.Enums;
using BulkMailSender.Domain.ValueObjects;
using BulkMailSender.Infrastructure.Common.Entities.Email;

namespace BulkMailSender.Infrastructure.Mappings {
    public class InfrastructureMappingProfile : Profile {
        public InfrastructureMappingProfile() {
            CreateMap<Email, EmailEntity>()
    .ForMember(dest => dest.EmailFrom, opt => opt.MapFrom(src => src.EmailFrom.Value))
    .ForMember(dest => dest.EmailTo, opt => opt.MapFrom(src => src.EmailTo.Value))
    .ForMember(dest => dest.EmailAttachments, opt => opt.Ignore())
    .ReverseMap() // Infrastructure to Domain Mapping
    .ForMember(dest => dest.EmailFrom, opt => opt.MapFrom(src => new EmailAddress(src.EmailFrom)))
    .ForMember(dest => dest.EmailTo, opt => opt.MapFrom(src => new EmailAddress(src.EmailTo)))
  .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId)); // Direct StatusId mapping

            CreateMap<Attachment, AttachmentEntity>();
            CreateMap<InlineResource, InlineResourceEntity>();

 
            // Map from RequesterEntity to Requester (Domain Model)
            CreateMap<RequesterEntity, Requester>()
                .ConstructUsing(src => new Requester(src.LoginName, src.Password, new MailServer(
                    src.MailServer.ServerName,
                    src.MailServer.Port,
                    src.MailServer.IsSecure
                )));

            // Domain to Entity Mapping
            CreateMap<Requester, RequesterEntity>()
                .ForMember(dest => dest.MailServerId, opt => opt.MapFrom(src => src.MailServerId))
                .ForMember(dest => dest.MailServer, opt => opt.Ignore()) // Ignored here, map separately if needed
                .ConstructUsing(src => new RequesterEntity {
                    Id = src.Id,
                    LoginName = src.LoginName,
                    Password = src.Password,
                    MailServerId = src.MailServerId
                });
            // Map from MailServerEntity to MailServer (Domain Model)
            CreateMap<MailServerEntity, MailServer>();

            CreateMap<StatusEntity, Status>()
                 .ConvertUsing(src => (Status)src.Id);

            // Map Status (enum) to StatusEntity
            CreateMap<Status, StatusEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (int)src))
                .ForMember(dest => dest.Name, opt => opt.Ignore());
        }
    }
}
