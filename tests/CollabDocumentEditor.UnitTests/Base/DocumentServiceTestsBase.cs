using AutoMapper;
using CollabDocumentEditor.Core.Dtos;
using CollabDocumentEditor.Core.Interfaces.Repositories;
using CollabDocumentEditor.Core.Interfaces.Services;
using CollabDocumentEditor.Infrastructure.Services;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace CollabDocumentEditor.UnitTests.Base;

public class DocumentServiceTestsBase
{
   protected readonly Mock<IDocumentRepository> DocumentRepositoryMock;
   protected readonly Mock<ILogger<DocumentService>> LoggerMock;
   protected readonly Mock<IMapper> MapperMock;
   protected readonly Mock<ICurrentUserService> CurrentUserServiceMock;
   protected readonly Mock<IDocumentPermissionRepository> DocumentPermissionRepositoryMock;
   protected readonly Mock<IValidator<CreateDocumentDto>> CreateDocumentValidatorMock;
   protected readonly Mock<IValidator<UpdateDocumentDto>> UpdateDocumentValidatorMock;
   protected readonly Mock<IValidator<ShareDocumentDto>> ShareDocumentValidatorMock;
   protected readonly DocumentService DocumentServiceMock;
   
   protected DocumentServiceTestsBase()
   {
       DocumentRepositoryMock = new Mock<IDocumentRepository>();
       LoggerMock = new Mock<ILogger<DocumentService>>();
       MapperMock = new Mock<IMapper>();
       CurrentUserServiceMock = new Mock<ICurrentUserService>();
       DocumentPermissionRepositoryMock = new Mock<IDocumentPermissionRepository>();
       CreateDocumentValidatorMock = new Mock<IValidator<CreateDocumentDto>>();
       UpdateDocumentValidatorMock = new Mock<IValidator<UpdateDocumentDto>>();
       ShareDocumentValidatorMock = new Mock<IValidator<ShareDocumentDto>>();
       
       DocumentServiceMock = new DocumentService(
           DocumentRepositoryMock.Object,
           LoggerMock.Object,
           MapperMock.Object,
           CurrentUserServiceMock.Object,
           DocumentPermissionRepositoryMock.Object,
           CreateDocumentValidatorMock.Object,
           UpdateDocumentValidatorMock.Object,
           ShareDocumentValidatorMock.Object
       );
   }
}