using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Application.Extensions;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Domain.Entities;
using AgendamentoConsultas.Domain.Patterns;

namespace AgendamentoConsultas.Application.UseCases.Cliente;

public class CriarClienteUseCase : ICriarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;

    public CriarClienteUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<Result<ClienteDto>> ExecutarAsync(CriarClienteDto dto)
    {
        var clienteExistente = await _clienteRepository.ObterPorCpfAsync(dto.Cpf);
        if (clienteExistente != null)
            return Result.Failure<ClienteDto>("Já existe um cliente cadastrado com este CPF");

        var clienteResult = Domain.Entities.Cliente.Criar(
            dto.Nome,
            dto.Cpf,
            dto.Email,
            dto.Telefone,
            dto.DataNascimento
        );

        if (clienteResult.IsFailure)
            return Result.Failure<ClienteDto>(clienteResult.Error);

        await _clienteRepository.AdicionarAsync(clienteResult.Value);

        return Result.Success(clienteResult.Value.ToDto());
    }
}
