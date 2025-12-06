namespace Applocation.Wrappers;

public class ResponseWrapper : IResponseWrapper
{
    public List<string> Message { get; set; } = [];
    public bool IsSuccessful { get; set; }

    public ResponseWrapper()
    {
    }

    public static IResponseWrapper Fail()
    {
        return new ResponseWrapper()
        {
            IsSuccessful = false,
        };
    }

    public static IResponseWrapper Fail(string message)
    {
        return new ResponseWrapper()
        {
            IsSuccessful = false,
            Message = [message]
        };
    }

    public static IResponseWrapper Fail(List<string> message)
    {
        return new ResponseWrapper()
        {
            IsSuccessful = false,
            Message = message
        };
    }

    public static Task<IResponseWrapper> FailAsync()
    {
        return Task.FromResult(Fail());
    }

    public static Task<IResponseWrapper> FailAsync(string message)
    {
        return Task.FromResult(Fail(message));
    }

    public static Task<IResponseWrapper> FailAsync(List<string> message)
    {
        return Task.FromResult(Fail(message));
    }

    public static IResponseWrapper Success()
    {
        return new ResponseWrapper()
        {
            IsSuccessful = true,
        };
    }

    public static IResponseWrapper Success(string message)
    {
        return new ResponseWrapper()
        {
            IsSuccessful = true,
            Message = [message]
        };
    }

    public static IResponseWrapper Success(List<string> message)
    {
        return new ResponseWrapper()
        {
            IsSuccessful = true,
            Message = message
        };
    }

    public static Task<IResponseWrapper> SuccessAsync()
    {
        return Task.FromResult(Success());
    }

    public static Task<IResponseWrapper> SuccessAsync(string message)
    {
        return Task.FromResult(Success(message));
    }

    public static Task<IResponseWrapper> SuccessAsync(List<string> message)
    {
        return Task.FromResult(Success(message));
    }
}

public class ResponseWrapper<T> : ResponseWrapper, IResponseWrapper<T>
{
    public ResponseWrapper()
    {
    }

    public T Data { get; set; }

    public new static ResponseWrapper<T> Fail()
    {
        return new ResponseWrapper<T>
        {
            IsSuccessful = false,
        };
    }

    public new static ResponseWrapper<T> Fail(string message)
    {
        return new ResponseWrapper<T>
        {
            IsSuccessful = false,
            Message = [message]
        };
    }

    public new static ResponseWrapper<T> Fail(List<string> message)
    {
        return new ResponseWrapper<T>
        {
            IsSuccessful = false,
            Message = message
        };
    }

    public new static Task<ResponseWrapper<T>> FailAsync()
    {
        return Task.FromResult(Fail());
    }

    public new static Task<ResponseWrapper<T>> FailAsync(string message)
    {
        return Task.FromResult(Fail(message));
    }

    public new static Task<ResponseWrapper<T>> FailAsync(List<string> message)
    {
        return Task.FromResult(Fail(message));
    }

    public new static ResponseWrapper<T> Success()
    {
        return new ResponseWrapper<T>
        {
            IsSuccessful = true,
        };
    }

    public new static ResponseWrapper<T> Success(string message)
    {
        return new ResponseWrapper<T>
        {
            IsSuccessful = true,
            Message = [message]
        };
    }

    public new static ResponseWrapper<T> Success(List<string> message)
    {
        return new ResponseWrapper<T>
        {
            IsSuccessful = true,
            Message = message
        };
    }

    public new static Task<ResponseWrapper<T>> SuccessAsync()
    {
        return Task.FromResult(Success());
    }

    public new static Task<ResponseWrapper<T>> SuccessAsync(string message)
    {
        return Task.FromResult(Success(message));
    }

    public new static Task<ResponseWrapper<T>> SuccessAsync(List<string> message)
    {
        return Task.FromResult(Success(message));
    }
}