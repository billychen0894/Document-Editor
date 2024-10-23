using AutoMapper;
using CollabDocumentEditor.Core.Dtos;
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
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                srcMember != null));
    }
}