using CollabDocumentEditor.Core.Common;
using CollabDocumentEditor.Core.Dtos;
using CollabDocumentEditor.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabDocumentEditor.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/documents")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly ILogger<DocumentsController> _logger;
    
    public DocumentsController(IDocumentService documentService, ILogger<DocumentsController> logger)
    {
        _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    [HttpGet("{documentId}", Name = "GetDocument")]
    [Authorize(Policy = "DocumentRole")]
    public async Task<IActionResult> GetDocumentAsync(Guid documentId)
    {
        if (documentId == Guid.Empty)
        {
            return BadRequest("Document id cannot be empty");
        }
        
        var document = await _documentService.GetDocumentAsync(documentId);
        return Ok(document);
    }

    [HttpGet("user/{userId}")]
    [Authorize(Policy="DocumentRole")]
    public async Task<IActionResult> GetUserDocumentsAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("User id cannot be empty");
        }
        
        var documents = await _documentService.GetUserDocumentsAsync(userId);
        return Ok(documents);
    }

    [HttpPost]
    [Authorize(Policy = "DocumentRole")]
    public async Task<IActionResult> CreateDocumentAsync([FromBody] CreateDocumentDto dto)
    {
        var document = await _documentService.CreateDocumentAsync(dto);
        
        _logger.LogInformation("Document created: {document}", document);
        
        return CreatedAtRoute(
            routeName: "GetDocument",
            routeValues: new { documentId = document.Value.Id },
            value: document
        );
    }
    
    [HttpPut("{documentId}")]
    [Authorize(Policy="DocumentRole")]
    public async Task<IActionResult> UpdateDocumentAsync(Guid documentId, [FromBody] UpdateDocumentDto dto)
    {
        if (documentId == Guid.Empty)
        {
            return BadRequest("Document id cannot be empty");
        }
        
        if (documentId != dto.Id)
        {
            _logger.LogWarning("Document ID mismatch. URL ID: {UrlId}, DTO ID: {DtoId}", documentId, dto.Id);
            return BadRequest("Document id in the request body does not match the document id in the URL");
        }
        
        var document = await _documentService.UpdateDocumentAsync(dto);
        
        _logger.LogInformation("Document updated: {document}", document);
        return Ok(document);
    }
    
    [HttpDelete("{documentId}")]
    [Authorize(Policy="DocumentRole")]
    public async Task<IActionResult> DeleteDocumentAsync(Guid documentId)
    {
        if(documentId == Guid.Empty)
        {
            return BadRequest("Document id cannot be empty");
        }
        
        await _documentService.DeleteDocumentAsync(documentId);
        
        _logger.LogInformation("Document deleted: {documentId}", documentId);
        return NoContent();
    }
    
    [HttpPost("share")]
    [Authorize(Policy="DocumentRole")]
    public async Task<IActionResult> ShareDocumentAsync([FromBody] ShareDocumentDto dto)
    {
        await _documentService.ShareDocumentAsync(dto);
        
        _logger.LogInformation("Document shared: {documentId}", dto.DocumentId);
        return NoContent();
    }
}