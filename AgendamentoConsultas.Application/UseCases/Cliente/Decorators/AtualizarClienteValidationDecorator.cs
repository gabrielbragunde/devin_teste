using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Domain.Patterns;
using FluentValidation;

namespace AgendamentoConsultas.Application.UseCases.Cliente;

public class AtualizarClienteValidationDecorator : IAtualizarClienteUseCase
{
    private readonly IAtualizarClienteUseCase _inner;
    private readonly IValidator<AtualizarClienteDto> _validator;

    public AtualizarClienteValidationDecorator(
        IAtualizarClienteUseCase inner,
        IValidator<AtualizarClienteDto> validator)
    {
        _inner = inner;
        _validator = validator;
    }

    public async Task<Result<ClienteDto>> ExecutarAsync(Guid id, AtualizarClienteDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure<ClienteDto>(errors);
        }

        return await _inner.ExecutarAsync(id, dto);
    }
}
