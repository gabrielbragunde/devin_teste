using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.Extensions;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Application.Validators.Agendamento;
using AgendamentoConsultas.Domain.Patterns;

namespace AgendamentoConsultas.Application.UseCases.Agendamento;

public class ReagendarUseCase
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly ReagendarDtoValidator _validator;

    public ReagendarUseCase(
        IAgendamentoRepository agendamentoRepository,
        IClienteRepository clienteRepository)
    {
        _agendamentoRepository = agendamentoRepository;
        _clienteRepository = clienteRepository;
        _validator = new ReagendarDtoValidator();
    }

    public async Task<Result<AgendamentoDto>> ExecutarAsync(Guid id, ReagendarDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure<AgendamentoDto>(errors);
        }

        var agendamento = await _agendamentoRepository.ObterPorIdAsync(id);
        if (agendamento == null)
            return Result.Failure<AgendamentoDto>("Agendamento não encontrado");

        var horarioOcupado = await _agendamentoRepository.ExisteAgendamentoNoHorarioAsync(dto.NovaDataHora);
        if (horarioOcupado)
            return Result.Failure<AgendamentoDto>("Já existe um agendamento neste horário");

        var reagendarResult = agendamento.Reagendar(dto.NovaDataHora);
        if (reagendarResult.IsFailure)
            return Result.Failure<AgendamentoDto>(reagendarResult.Error);
        
        var sucesso = await _agendamentoRepository.AtualizarAsync(agendamento);
        
        if (!sucesso)
            return Result.Failure<AgendamentoDto>("Falha ao atualizar agendamento no repositório");

        var cliente = await _clienteRepository.ObterPorIdAsync(agendamento.ClienteId);

        return Result.Success(agendamento.ToDto(cliente?.Nome ?? "Cliente não encontrado"));
    }
}
