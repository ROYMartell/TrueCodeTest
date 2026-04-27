using FluentValidation;
using MediatR;
using TrueCodeTest.Shared.Kernel;

namespace TrueCodeTest.Shared.Mediator;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = results.SelectMany(r => r.Errors).Where(f => f is not null).ToList();

        if (failures.Count == 0)
            return await next();

        var message = string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
        var error = Error.Validation("validation.failed", message);

        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(error);

        var responseType = typeof(TResponse);
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var method = typeof(Result)
                .GetMethods()
                .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethod)
                .MakeGenericMethod(valueType);
            return (TResponse)method.Invoke(null, new object[] { error })!;
        }

        throw new ValidationException(failures);
    }
}
