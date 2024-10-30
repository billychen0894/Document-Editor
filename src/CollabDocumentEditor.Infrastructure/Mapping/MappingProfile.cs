using AutoMapper;
using CollabDocumentEditor.Core.Dtos;
using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.Core.Entities;
using Document = CollabDocumentEditor.Core.Entities.Document;

namespace CollabDocumentEditor.Infrastructure.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Document -> DocumentDto
        CreateMap<Document, DocumentDto>();
        
        // CreateDocumentDto -> Document
        CreateMap<CreateDocumentDto, Document>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // UpdateDocumentDto -> Document
        CreateMap<UpdateDocumentDto, Document>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                srcMember != null));
        
        // RegisterDto -> ApplicationUser
        CreateMap<RegisterDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));
    }
}