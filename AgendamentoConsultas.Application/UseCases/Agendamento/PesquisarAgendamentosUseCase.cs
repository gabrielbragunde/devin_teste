using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.Extensions;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Domain.Entities;
using AgendamentoConsultas.Domain.Patterns;

namespace AgendamentoConsultas.Application.UseCases.Agendamento;

public class PesquisarAgendamentosUseCase
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IClienteRepository _clienteRepository;

    public PesquisarAgendamentosUseCase(
        IAgendamentoRepository agendamentoRepository,
        IClienteRepository clienteRepository)
    {
        _agendamentoRepository = agendamentoRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<Result<IEnumerable<AgendamentoDto>>> ExecutarAsync(PesquisarAgendamentosFiltroDto filtro)
    {
        if (filtro.DataInicio.HasValue && filtro.DataFim.HasValue && filtro.DataInicio > filtro.DataFim)
        {
            return Result.Failure<IEnumerable<AgendamentoDto>>("Período de datas inválido: DataInicio > DataFim");
        }

        StatusAgendamento? status = null;
        if (!string.IsNullOrWhiteSpace(filtro.Status))
        {
            if (!Enum.TryParse<StatusAgendamento>(filtro.Status, true, out var parsed))
            {
                return Result.Failure<IEnumerable<AgendamentoDto>>("Status inválido. Valores aceitos: Agendado, Concluido, Cancelado");
            }
            status = parsed;
        }

        if (filtro.ClienteId.HasValue)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(filtro.ClienteId.Value);
            if (cliente == null)
                return Result.Failure<IEnumerable<AgendamentoDto>>("Cliente não encontrado");
        }

        var agendamentos = await _agendamentoRepository.PesquisarAsync(
            filtro.ClienteId,
            filtro.DataInicio,
            filtro.DataFim,
            status,
            filtro.Texto);

        var clienteIds = agendamentos
            .Select(a => a.ClienteId)
            .Distinct()
            .ToList();

        var clienteIdToNome = new Dictionary<Guid, string>();
        foreach (var clienteId in clienteIds)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(clienteId);
            if (cliente != null)
            {
                clienteIdToNome[clienteId] = cliente.Nome;
            }
        }

        var dtos = agendamentos
            .Select(a =>
            {
                var nome = clienteIdToNome.TryGetValue(a.ClienteId, out var n) ? n : string.Empty;
                return a.ToDto(nome);
            })
            .ToList();

        return Result.Success<IEnumerable<AgendamentoDto>>(dtos);
    }
}
