using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.Interfaces;

namespace AgendamentoConsultas.Application.UseCases.Agendamento;

public class CriarAgendamentoUseCase
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IClienteRepository _clienteRepository;

    public CriarAgendamentoUseCase(
        IAgendamentoRepository agendamentoRepository,
        IClienteRepository clienteRepository)
    {
        _agendamentoRepository = agendamentoRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<AgendamentoDto> ExecutarAsync(CriarAgendamentoDto dto)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(dto.ClienteId);
        if (cliente == null)
            throw new InvalidOperationException("Cliente não encontrado");

        var horarioOcupado = await _agendamentoRepository.ExisteAgendamentoNoHorarioAsync(dto.DataHora);
        if (horarioOcupado)
            throw new InvalidOperationException("Já existe um agendamento neste horário");

        var agendamento = new Domain.Entities.Agendamento(
            dto.ClienteId,
            dto.DataHora,
            dto.Observacoes
        );

        await _agendamentoRepository.AdicionarAsync(agendamento);

        return new AgendamentoDto(
            agendamento.Id,
            agendamento.ClienteId,
            cliente.Nome,
            agendamento.DataHora,
            agendamento.Status.ToString(),
            agendamento.Observacoes,
            agendamento.CriadoEm,
            agendamento.AtualizadoEm
        );
    }
}
