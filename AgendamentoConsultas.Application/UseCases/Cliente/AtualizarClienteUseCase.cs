using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Application.Extensions;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Domain.Patterns;

namespace AgendamentoConsultas.Application.UseCases.Cliente;

public class AtualizarClienteUseCase : IAtualizarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;

    public AtualizarClienteUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<Result<ClienteDto>> ExecutarAsync(Guid id, AtualizarClienteDto dto)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(id);
        
        if (cliente == null)
            return Result.Failure<ClienteDto>("Cliente não encontrado");

        var atualizacaoResult = cliente.Atualizar(dto.Nome, dto.Email, dto.Telefone, dto.DataNascimento);
        
        if (atualizacaoResult.IsFailure)
            return Result.Failure<ClienteDto>(atualizacaoResult.Error);
        
        var sucesso = await _clienteRepository.AtualizarAsync(cliente);
        
        if (!sucesso)
            return Result.Failure<ClienteDto>("Falha ao atualizar cliente no repositório");

        return Result.Success(cliente.ToDto());
    }
}
