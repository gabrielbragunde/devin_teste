using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Application.Extensions;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Application.Validators.Cliente;
using AgendamentoConsultas.Domain.Patterns;

namespace AgendamentoConsultas.Application.UseCases.Cliente;

public class AtualizarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly AtualizarClienteDtoValidator _validator;

    public AtualizarClienteUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
        _validator = new AtualizarClienteDtoValidator();
    }

    public async Task<Result<ClienteDto>> ExecutarAsync(Guid id, AtualizarClienteDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure<ClienteDto>(errors);
        }

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
