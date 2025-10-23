using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Domain.Patterns;

namespace AgendamentoConsultas.Application.UseCases.Cliente;

public interface IAtualizarClienteUseCase
{
    Task<Result<ClienteDto>> ExecutarAsync(Guid id, AtualizarClienteDto dto);
}
