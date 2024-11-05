using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.Core.Exceptions;
using CollabDocumentEditor.Core.Interfaces.Repositories;
using CollabDocumentEditor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;

namespace CollabDocumentEditor.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DocumentRepository> _logger;
    
    public DocumentRepository(ApplicationDbContext context, ILogger<DocumentRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Document> GetByIdAsync(Guid documentId)
    {
        try
        {
            if (documentId == Guid.Empty) throw new ArgumentNullException(nameof(documentId));

            var document = await _context.Documents.FindAsync(documentId);
            if (document == null)
            {
                _logger.LogWarning("Document with id {DocumentId} not found", documentId);
                throw new NotFoundException($"Document with id {documentId} not found");
            }
            
            return document;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error while getting document with id {DocumentId}", documentId);
            throw new RepositoryException("Error occurred while retrieving the document.", ex);
        }
    }

    public async Task<IEnumerable<Document>> GetUserDocumentsAsync(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

            return await _context.Documents
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
           _logger.LogError(ex, "Error while getting documents for user {UserId}", userId); 
           throw new RepositoryException("Error occurred while retrieving user documents.", ex);
        }
    }

    public async Task<Document> CreateAsync(Document document)
    {
        try
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            
            await _context.Documents.AddAsync(document);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created document with id {DocumentId}", document.Id);
            return document;
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error while creating document {Document}", document);  
          throw new RepositoryException("Error occurred while creating document.", ex);
        }
    }

    public async Task<Document> UpdateAsync(Document document)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(document);

            var existingDocument = await GetByIdAsync(document.Id);
            if (existingDocument == null)
            {
                _logger.LogWarning("Document with id {DocumentId} not found", document.Id);
                throw new NotFoundException($"Document with id {document.Id} not found");
            }
            
            _context.Entry(existingDocument).CurrentValues.SetValues(document);
            await _context.SaveChangesAsync();

            return existingDocument;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
           _logger.LogError(ex, "Error while updating document {Document}", document); 
           throw new RepositoryException("Error occurred while updating document.", ex);
        }
    }

    public async Task<Document> DeleteAsync(Guid documentId)
    {
        try
        {
            if (documentId == Guid.Empty) throw new ArgumentNullException(nameof(documentId));
            
            var existingDocument = await GetByIdAsync(documentId);
            if (existingDocument == null)
            {
                _logger.LogWarning("Document with id {DocumentId} not found", documentId);
                throw new NotFoundException($"Document with id {documentId} not found");
            }
            
            _logger.LogInformation("Successfully deleted document with id {DocumentId}", documentId);
            
            _context.Documents.Remove(existingDocument);
            await _context.SaveChangesAsync();

            return existingDocument;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error while deleting document {DocumentId}", documentId);
            throw new RepositoryException("Error occurred while deleting document.", ex);
        }
    }
}