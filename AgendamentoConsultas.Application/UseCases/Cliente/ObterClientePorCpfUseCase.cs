using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Application.Interfaces;

namespace AgendamentoConsultas.Application.UseCases.Cliente;

public class ObterClientePorCpfUseCase
{
    private readonly IClienteRepository _clienteRepository;

    public ObterClientePorCpfUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteDto?> ExecutarAsync(string cpf)
    {
        var cliente = await _clienteRepository.ObterPorCpfAsync(cpf);
        
        if (cliente == null)
            return null;

        return new ClienteDto(
            cliente.Id,
            cliente.Nome,
            cliente.Cpf,
            cliente.Email,
            cliente.Telefone,
            cliente.DataNascimento,
            cliente.CriadoEm,
            cliente.AtualizadoEm
        );
    }
}
