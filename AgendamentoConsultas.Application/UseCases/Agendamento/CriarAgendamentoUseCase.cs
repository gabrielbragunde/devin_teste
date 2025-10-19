using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.Extensions;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Domain.Patterns;

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

    public async Task<Result<AgendamentoDto>> ExecutarAsync(CriarAgendamentoDto dto)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(dto.ClienteId);
        if (cliente == null)
            return Result.Failure<AgendamentoDto>("Cliente não encontrado");

        var horarioOcupado = await _agendamentoRepository.ExisteAgendamentoNoHorarioAsync(dto.DataHora);
        if (horarioOcupado)
            return Result.Failure<AgendamentoDto>("Já existe um agendamento neste horário");

        var agendamentoResult = Domain.Entities.Agendamento.Criar(
            dto.ClienteId,
            dto.DataHora,
            dto.Observacoes
        );

        if (agendamentoResult.IsFailure)
            return Result.Failure<AgendamentoDto>(agendamentoResult.Error);

        await _agendamentoRepository.AdicionarAsync(agendamentoResult.Value);

        return Result.Success(agendamentoResult.Value.ToDto(cliente.Nome));
    }
}
