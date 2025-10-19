using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Application.Interfaces;

namespace AgendamentoConsultas.Application.UseCases.Cliente;

public class AtualizarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;

    public AtualizarClienteUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteDto> ExecutarAsync(Guid id, AtualizarClienteDto dto)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(id);
        
        if (cliente == null)
            throw new InvalidOperationException("Cliente não encontrado");

        cliente.Atualizar(dto.Nome, dto.Email, dto.Telefone, dto.DataNascimento);
        
        await _clienteRepository.AtualizarAsync(cliente);

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
