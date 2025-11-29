using System.Net;

namespace Applocation.Exceptions;

public class NotFoundException : Exception
{
    public List<string> ErrorMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public NotFoundException(List<string> errorMessage = default, HttpStatusCode statusCode = HttpStatusCode.NotFound)
    {
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }
}