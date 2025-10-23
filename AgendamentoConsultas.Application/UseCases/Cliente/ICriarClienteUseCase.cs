using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Domain.Patterns;

namespace AgendamentoConsultas.Application.UseCases.Cliente;

public interface ICriarClienteUseCase
{
    Task<Result<ClienteDto>> ExecutarAsync(CriarClienteDto dto);
}
