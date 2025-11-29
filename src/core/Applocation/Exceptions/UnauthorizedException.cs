using System.Net;

namespace Applocation.Exceptions;

public class UnauthorizedException : Exception
{
    public List<string> ErrorMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public UnauthorizedException(List<string> errorMessage = default, HttpStatusCode statusCode = HttpStatusCode.Unauthorized)
    {
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }
}