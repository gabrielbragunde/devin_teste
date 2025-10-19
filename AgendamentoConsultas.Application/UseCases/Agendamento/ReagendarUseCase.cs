using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.Extensions;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Domain.Patterns;

namespace AgendamentoConsultas.Application.UseCases.Agendamento;

public class ReagendarUseCase
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IClienteRepository _clienteRepository;

    public ReagendarUseCase(
        IAgendamentoRepository agendamentoRepository,
        IClienteRepository clienteRepository)
    {
        _agendamentoRepository = agendamentoRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<Result<AgendamentoDto>> ExecutarAsync(Guid id, ReagendarDto dto)
    {
        var agendamento = await _agendamentoRepository.ObterPorIdAsync(id);
        if (agendamento == null)
            return Result.Failure<AgendamentoDto>("Agendamento não encontrado");

        var horarioOcupado = await _agendamentoRepository.ExisteAgendamentoNoHorarioAsync(dto.NovaDataHora);
        if (horarioOcupado)
            return Result.Failure<AgendamentoDto>("Já existe um agendamento neste horário");

        var reagendarResult = agendamento.Reagendar(dto.NovaDataHora);
        if (reagendarResult.IsFailure)
            return Result.Failure<AgendamentoDto>(reagendarResult.Error);
        
        await _agendamentoRepository.AtualizarAsync(agendamento);

        var cliente = await _clienteRepository.ObterPorIdAsync(agendamento.ClienteId);

        return Result.Success(agendamento.ToDto(cliente?.Nome ?? "Cliente não encontrado"));
    }
}
