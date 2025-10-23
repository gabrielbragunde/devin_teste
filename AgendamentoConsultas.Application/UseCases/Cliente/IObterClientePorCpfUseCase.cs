using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Domain.Patterns;

namespace AgendamentoConsultas.Application.UseCases.Cliente;

public interface IObterClientePorCpfUseCase
{
    Task<Result<ClienteDto>> ExecutarAsync(string cpf);
}
