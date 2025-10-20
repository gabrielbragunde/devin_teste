using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.Extensions;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Domain.Patterns;

namespace AgendamentoConsultas.Application.UseCases.Agendamento;

public class CancelarAgendamentoUseCase
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IClienteRepository _clienteRepository;

    public CancelarAgendamentoUseCase(
        IAgendamentoRepository agendamentoRepository,
        IClienteRepository clienteRepository)
    {
        _agendamentoRepository = agendamentoRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<Result<AgendamentoDto>> ExecutarAsync(Guid id)
    {
        var agendamento = await _agendamentoRepository.ObterPorIdAsync(id);
        if (agendamento == null)
            return Result.Failure<AgendamentoDto>("Agendamento não encontrado");

        var cancelarResult = agendamento.Cancelar();
        if (cancelarResult.IsFailure)
            return Result.Failure<AgendamentoDto>(cancelarResult.Error);
        
        var sucesso = await _agendamentoRepository.AtualizarAsync(agendamento);
        
        if (!sucesso)
            return Result.Failure<AgendamentoDto>("Falha ao atualizar agendamento no repositório");

        var cliente = await _clienteRepository.ObterPorIdAsync(agendamento.ClienteId);

        return Result.Success(agendamento.ToDto(cliente?.Nome ?? "Cliente não encontrado"));
    }
}
