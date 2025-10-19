using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Application.Extensions;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Domain.Patterns;

namespace AgendamentoConsultas.Application.UseCases.Cliente;

public class ObterClientePorCpfUseCase
{
    private readonly IClienteRepository _clienteRepository;

    public ObterClientePorCpfUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<Result<ClienteDto>> ExecutarAsync(string cpf)
    {
        var cliente = await _clienteRepository.ObterPorCpfAsync(cpf);
        
        if (cliente == null)
            return Result.Failure<ClienteDto>("Cliente não encontrado");

        return Result.Success(cliente.ToDto());
    }
}
