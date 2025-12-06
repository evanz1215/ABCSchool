namespace Applocation.Wrappers;

public interface IResponseWrapper
{
    List<string> Message { get; set; }
    bool IsSuccessful { get; set; }
}

public interface IResponseWrapper<out T> : IResponseWrapper
{
    T Data { get; }
}