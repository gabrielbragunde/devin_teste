using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.Interfaces;

namespace AgendamentoConsultas.Application.UseCases.Agendamento;

public class ObterAgendamentosPorClienteUseCase
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IClienteRepository _clienteRepository;

    public ObterAgendamentosPorClienteUseCase(
        IAgendamentoRepository agendamentoRepository,
        IClienteRepository clienteRepository)
    {
        _agendamentoRepository = agendamentoRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<IEnumerable<AgendamentoDto>> ExecutarAsync(Guid clienteId)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(clienteId);
        if (cliente == null)
            throw new InvalidOperationException("Cliente não encontrado");

        var agendamentos = await _agendamentoRepository.ObterPorClienteIdAsync(clienteId);

        return agendamentos.Select(a => new AgendamentoDto(
            a.Id,
            a.ClienteId,
            cliente.Nome,
            a.DataHora,
            a.Status.ToString(),
            a.Observacoes,
            a.CriadoEm,
            a.AtualizadoEm
        ));
    }
}
