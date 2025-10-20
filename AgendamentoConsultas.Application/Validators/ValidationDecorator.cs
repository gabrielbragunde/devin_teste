using AgendamentoConsultas.Domain.Patterns;
using FluentValidation;

namespace AgendamentoConsultas.Application.Validators;

public class ValidationDecorator<TRequest, TResponse>
{
    private readonly IValidator<TRequest> _validator;

    public ValidationDecorator(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async Task<Result<TResponse>> ValidateAsync(TRequest request, Func<TRequest, Task<Result<TResponse>>> next)
    {
        var validationResult = await _validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure<TResponse>(errors);
        }

        return await next(request);
    }
}
