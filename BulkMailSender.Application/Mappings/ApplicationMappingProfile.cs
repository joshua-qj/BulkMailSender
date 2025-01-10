using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Domain.Entities.Email;
using BulkMailSender.Domain.Entities.Identity;
using BulkMailSender.Domain.ValueObjects;

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
                   .ForMember(dest => dest.Status, opt => opt.Ignore());
 
            CreateMap<Email, EmailDto>()
                .ForMember(dest => dest.EmailFrom, opt => opt.MapFrom(src => src.EmailFrom.Value))
                .ForMember(dest => dest.EmailTo, opt => opt.MapFrom(src => src.EmailTo.Value))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
                .ForMember(dest => dest.InlineResources, opt => opt.MapFrom(src => src.InlineResources));


            CreateMap<Attachment, AttachmentDto>().ReverseMap(); 
            CreateMap<InlineResource, InlineResourceDto>().ReverseMap();






            CreateMap<UserDto, User>()
    .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Ignore password hash for security reasons
   

            // Optional: Configure mapping from User to UserDto (if needed)
            CreateMap<User, UserDto>();

            CreateMap<Claim, System.Security.Claims.Claim>()
             .ConstructUsing(src => new System.Security.Claims.Claim(src.Type, src.Value));
            CreateMap<System.Security.Claims.Claim,Claim>()
              .ConstructUsing(src => new Claim(src.Type, src.Value));

            CreateMap<Result, ResultDto>();
            CreateMap<ResultDto, Result>();
        }

    }
}
