using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.Extensions;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Domain.Patterns;

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

    public async Task<Result<IEnumerable<AgendamentoDto>>> ExecutarAsync(Guid clienteId)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(clienteId);
        if (cliente == null)
            return Result.Failure<IEnumerable<AgendamentoDto>>("Cliente não encontrado");

        var agendamentos = await _agendamentoRepository.ObterPorClienteIdAsync(clienteId);

        var agendamentosDto = agendamentos.Select(a => a.ToDto(cliente.Nome));
        
        return Result.Success(agendamentosDto);
    }
}
