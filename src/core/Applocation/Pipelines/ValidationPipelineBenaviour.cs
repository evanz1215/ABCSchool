using Applocation.Wrappers;
using FluentValidation;
using MediatR;

namespace Applocation.Pipelines;

public class ValidationPipelineBenaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
   where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBenaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task
                .WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            if (!validationResults.Any(x => x.IsValid))
            {
                List<string> errors = [];
                var failures = validationResults.SelectMany(x => x.Errors)
                    .Where(x => x != null).ToList();

                foreach (var failure in failures)
                {
                    errors.Add(failure.ErrorMessage);
                }

                return (TResponse)await ResponseWrapper.FailAsync(errors);
            }
        }

        return await next();
    }
}