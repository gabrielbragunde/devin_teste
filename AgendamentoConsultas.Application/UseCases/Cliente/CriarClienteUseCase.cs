using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Domain.Entities;

namespace AgendamentoConsultas.Application.UseCases.Cliente;

public class CriarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;

    public CriarClienteUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteDto> ExecutarAsync(CriarClienteDto dto)
    {
        var clienteExistente = await _clienteRepository.ObterPorCpfAsync(dto.Cpf);
        if (clienteExistente != null)
            throw new InvalidOperationException("Já existe um cliente cadastrado com este CPF");

        var cliente = new Domain.Entities.Cliente(
            dto.Nome,
            dto.Cpf,
            dto.Email,
            dto.Telefone,
            dto.DataNascimento
        );

        await _clienteRepository.AdicionarAsync(cliente);

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
