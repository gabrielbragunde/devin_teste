using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Domain.Patterns;
using FluentValidation;

namespace AgendamentoConsultas.Application.UseCases.Cliente;

public class CriarClienteValidationDecorator : ICriarClienteUseCase
{
    private readonly ICriarClienteUseCase _inner;
    private readonly IValidator<CriarClienteDto> _validator;

    public CriarClienteValidationDecorator(
        ICriarClienteUseCase inner,
        IValidator<CriarClienteDto> validator)
    {
        _inner = inner;
        _validator = validator;
    }

    public async Task<Result<ClienteDto>> ExecutarAsync(CriarClienteDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure<ClienteDto>(errors);
        }

        return await _inner.ExecutarAsync(dto);
    }
}
