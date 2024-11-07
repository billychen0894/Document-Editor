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
    
    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
    }
    
    [HttpGet("{documentId}", Name = "GetDocument")]
    [Authorize(Policy = "DocumentRole")]
    public async Task<IActionResult> GetDocumentAsync(Guid documentId)
    {
        var document = await _documentService.GetDocumentAsync(documentId);
        return Ok(document);
    }

    [HttpGet("user/{userId}")]
    [Authorize(Policy="DocumentRole")]
    public async Task<IActionResult> GetUserDocumentsAsync(Guid userId)
    {
        var documents = await _documentService.GetUserDocumentsAsync(userId);
        return Ok(documents);
    }

    [HttpPost]
    [Authorize(Policy = "DocumentRole")]
    public async Task<IActionResult> CreateDocumentAsync([FromBody] CreateDocumentDto dto)
    {
        var document = await _documentService.CreateDocumentAsync(dto);
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
        if (documentId != dto.Id)
        {
            return BadRequest("Document id in the request body does not match the document id in the URL");
        }
        
        var document = await _documentService.UpdateDocumentAsync(dto);
        return Ok(document);
    }
    
    [HttpDelete("{documentId}")]
    [Authorize(Policy="DocumentRole")]
    public async Task<IActionResult> DeleteDocumentAsync(Guid documentId)
    {
        await _documentService.DeleteDocumentAsync(documentId);
        return NoContent();
    }
    
    [HttpPost("share")]
    [Authorize(Policy="DocumentRole")]
    public async Task<IActionResult> ShareDocumentAsync([FromBody] ShareDocumentDto dto)
    {
        await _documentService.ShareDocumentAsync(dto);
        return NoContent();
    }
}