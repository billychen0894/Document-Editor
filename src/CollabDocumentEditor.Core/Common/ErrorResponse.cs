namespace CollabDocumentEditor.Core.Common;

public class ErrorResponse
{
   public string Message { get; set; } 
   public string[] Errors { get; set; }
   public int StatusCode { get; set; }

   public ErrorResponse(string message)
   {
      Message = message;
      Errors = Array.Empty<string>();
   }
   
   public ErrorResponse(string message, string[] errors)
   {
      Message = message;
      Errors = errors;
   }
}