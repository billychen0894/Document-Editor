using AutoMapper;
using CollabDocumentEditor.Core.Dtos;
using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.Core.Enum;
using CollabDocumentEditor.Core.Exceptions;
using CollabDocumentEditor.Core.Interfaces.Repositories;
using CollabDocumentEditor.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;

namespace CollabDocumentEditor.Infrastructure.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ILogger<DocumentService> _logger;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDocumentPermissionRepository _documentPermissionRepository;
    
    public DocumentService(
        IDocumentRepository documentRepository,
        ILogger<DocumentService> logger,
        IMapper mapper,
        ICurrentUserService currentUserService,
        IDocumentPermissionRepository documentPermissionRepository)
    {
        _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _documentPermissionRepository = documentPermissionRepository ?? throw new ArgumentNullException(nameof(documentPermissionRepository));
    }

    public async Task<DocumentDto> GetDocumentAsync(Guid documentId)
    {
        try
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null)
            {
                throw new NotFoundException($"Document with id {documentId} not found");
            }

            var authorizedResult = await _documentPermissionRepository.HasPermissionAsync(
                documentId,
                _currentUserService.UserId,
                DocumentRole.Viewer);

            if (!authorizedResult)
            {
               _logger.LogError("You do not have permission to access this document"); 
               throw new UnauthorizedAccessException("You do not have permission to access this document");
            }

            _logger.LogInformation("User {UserName} ({UserId}) retrieved document {DocumentId}",
                _currentUserService.UserName,
                _currentUserService.UserId,
                documentId);

            return _mapper.Map<DocumentDto>(document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving document {DocumentId}", documentId);
            throw;
        }
    }

    public async Task<IEnumerable<DocumentDto>> GetUserDocumentsAsync(Guid userId)
    {
        try
        {
            if (userId != _currentUserService.UserId && !_currentUserService.IsAdmin)
            {
                throw new UnauthorizedAccessException("You can only view your own documents");
            }

            var documents = await _documentRepository.GetUserDocumentsAsync(userId);
            var documentDtos = _mapper.Map<IEnumerable<DocumentDto>>(documents);

            var userDocumentsAsync = documentDtos.ToList();

            _logger.LogInformation("User {UserName} ({UserId}) retrieved {Count} documents for user {TargetUserId}",
                _currentUserService.UserName,
                _currentUserService.UserId,
                userDocumentsAsync.Count(),
                userId);

            return userDocumentsAsync;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "User {UserName} ({UserId}) attempted unauthorized access to documents of user {TargetUserId}", 
                _currentUserService.UserName, 
                _currentUserService.UserId, 
                userId);
            throw;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex, "Error occurred while user {UserName} ({UserId}) was retrieving documents for user {TargetUserId}", 
                _currentUserService.UserName, 
                _currentUserService.UserId, 
                userId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while user {UserName} ({UserId}) was retrieving documents for user {TargetUserId}", 
                _currentUserService.UserName, 
                _currentUserService.UserId, 
                userId);
            throw new ApplicationException("Error retrieving user documents", ex);
        }
    }

    public async Task<DocumentDto> CreateDocumentAsync(CreateDocumentDto dto)
    {
        try
        {
            var document = _mapper.Map<Document>(dto);
            document.UserId = _currentUserService.UserId;

            var createdDocument = await _documentRepository.CreateAsync(document);

            _logger.LogInformation("User {UserName} ({UserId}) created document {DocumentId}",
                _currentUserService.UserName,
                _currentUserService.UserId,
                createdDocument.Id);

            return _mapper.Map<DocumentDto>(createdDocument);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "User {UserName} ({UserId}) attempted to create document without proper permissions",
                _currentUserService.UserName,
                _currentUserService.UserId);
            throw;
        }
        catch (RepositoryException ex)
        {
            _logger.LogError(ex, "Error occurred while user {UserName} ({UserId}) was creating a document", 
                _currentUserService.UserName, 
                _currentUserService.UserId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while user {UserName} ({UserId}) was creating a document", 
                _currentUserService.UserName, 
                _currentUserService.UserId);
            throw new ApplicationException("Error creating document", ex);
        }
    }

    public async Task<DocumentDto> UpdateDocumentAsync(UpdateDocumentDto dto)
    {
        try
        {
            var existingDocument = await _documentRepository.GetByIdAsync(dto.Id);

            var authorizedResult = await _documentPermissionRepository.HasPermissionAsync(
                existingDocument.Id,
                _currentUserService.UserId,
                DocumentRole.Editor);

            if (!authorizedResult)
            {
               _logger.LogError("You do not have permission to update this document"); 
               throw new UnauthorizedAccessException("You do not have permission to update this document");
            }
            
            var documentWithUpdates = _mapper.Map<Document>(dto);
            documentWithUpdates.Id = existingDocument.Id;
            documentWithUpdates.UserId = existingDocument.UserId;
            documentWithUpdates.CreatedAt = existingDocument.CreatedAt;

            var updatedDocument = await _documentRepository.UpdateAsync(documentWithUpdates);

            _logger.LogInformation("User {UserName} ({UserId}) updated document {DocumentId}",
                _currentUserService.UserName,
                _currentUserService.UserId,
                dto.Id);

            return _mapper.Map<DocumentDto>(updatedDocument);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while user {UserName} ({UserId}) was updating document {DocumentId}", 
                _currentUserService.UserName, 
                _currentUserService.UserId, 
                dto.Id);
            throw;
        }
    }

    public async Task DeleteDocumentAsync(Guid documentId)
    {
        try
        {
            var document = await _documentRepository.GetByIdAsync(documentId);

            var authorizedResult = await _documentPermissionRepository.HasPermissionAsync(
                document.Id,
                _currentUserService.UserId,
                DocumentRole.Owner);

            if (!authorizedResult)
            {
               _logger.LogError("You do not have permission to delete this document"); 
               throw new UnauthorizedAccessException("You do not have permission to delete this document");
            }
            
            await _documentRepository.DeleteAsync(documentId);

            _logger.LogInformation("User {UserName} ({UserId}) deleted document {DocumentId}",
                _currentUserService.UserName,
                _currentUserService.UserId,
                documentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while user {UserName} ({UserId}) was deleting document {DocumentId}", 
                _currentUserService.UserName, 
                _currentUserService.UserId, 
                documentId);
            throw; 
        }
    }

    public async Task ShareDocumentAsync(ShareDocumentDto shareDocumentDto)
    {
        try
        {
            var document = await _documentRepository.GetByIdAsync(shareDocumentDto.DocumentId);

            // Check if current user can manage permissions
            var authorizedResult = await _documentPermissionRepository.HasPermissionAsync(
                document.Id,
                _currentUserService.UserId,
                DocumentRole.Viewer);

            if (!authorizedResult)
            {
               _logger.LogError("You do not have permission to share this document"); 
               throw new UnauthorizedAccessException("You do not have permission to share this document");
            }
            
            var permission = new DocumentUserPermission
            {
                DocumentId = shareDocumentDto.DocumentId,
                UserId = shareDocumentDto.UserId,
                Role = shareDocumentDto.Role,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = _currentUserService.UserId
            };

            await _documentPermissionRepository.GrantPermissionAsync(permission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while user {UserName} ({UserId}) was sharing document {DocumentId}", 
                _currentUserService.UserName, 
                _currentUserService.UserId, 
                shareDocumentDto.DocumentId);
            throw; 
        }
    }
}