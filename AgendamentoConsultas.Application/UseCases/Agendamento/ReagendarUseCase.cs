using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.Interfaces;

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

    public async Task<AgendamentoDto> ExecutarAsync(Guid id, ReagendarDto dto)
    {
        var agendamento = await _agendamentoRepository.ObterPorIdAsync(id);
        if (agendamento == null)
            throw new InvalidOperationException("Agendamento não encontrado");

        var horarioOcupado = await _agendamentoRepository.ExisteAgendamentoNoHorarioAsync(dto.NovaDataHora);
        if (horarioOcupado)
            throw new InvalidOperationException("Já existe um agendamento neste horário");

        agendamento.Reagendar(dto.NovaDataHora);
        await _agendamentoRepository.AtualizarAsync(agendamento);

        var cliente = await _clienteRepository.ObterPorIdAsync(agendamento.ClienteId);

        return new AgendamentoDto(
            agendamento.Id,
            agendamento.ClienteId,
            cliente?.Nome ?? "Cliente não encontrado",
            agendamento.DataHora,
            agendamento.Status.ToString(),
            agendamento.Observacoes,
            agendamento.CriadoEm,
            agendamento.AtualizadoEm
        );
    }
}
