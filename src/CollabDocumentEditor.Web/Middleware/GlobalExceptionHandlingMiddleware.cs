using System.Net;
using CollabDocumentEditor.Core.Common;
using CollabDocumentEditor.Core.Exceptions;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Errors.Model;

namespace CollabDocumentEditor.Web.Middleware;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
   private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger; 
   
   public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
   {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
   }
   
   public async Task InvokeAsync(HttpContext context, RequestDelegate next)
   {
      try
      {
         await next(context);
      }
      catch (Exception exception)
      {
         _logger.LogError(exception, exception.Message);

         var response = context.Response;
         response.ContentType = "application/json";

         var errorResponse = exception switch
         {
            RepositoryException ex => 
               (StatusCodes.Status500InternalServerError, new ErrorResponse(ex.Message)),
            
            NotFoundException ex => 
               (StatusCodes.Status404NotFound, new ErrorResponse(ex.Message)),
            
            EmailServiceException ex =>
               (StatusCodes.Status500InternalServerError, new ErrorResponse(ex.Message)),

            TokenValidationException ex =>
               (StatusCodes.Status401Unauthorized, new ErrorResponse(ex.Message)),
            
            UnauthorizedException ex =>
               (StatusCodes.Status403Forbidden, new ErrorResponse(ex.Message)),
            
            InvalidOperationException ex =>
               (StatusCodes.Status400BadRequest, new ErrorResponse(ex.Message)), 
            
            SecurityTokenException ex =>
               (StatusCodes.Status400BadRequest, new ErrorResponse(ex.Message)),
            
            ArgumentNullException ex =>
               (StatusCodes.Status400BadRequest, new ErrorResponse(ex.Message)),
            
            ApplicationException ex =>
               (StatusCodes.Status500InternalServerError, new ErrorResponse(ex.Message)),
            
            _ => (StatusCodes.Status500InternalServerError, new ErrorResponse("An error occurred while processing the request"))
         };
         
         response.StatusCode = errorResponse.Item1;
         await response.WriteAsJsonAsync(errorResponse.Item2);
      }
   }
}