using AgendamentoConsultas.Domain.Entities;

namespace AgendamentoConsultas.Application.Interfaces;

public interface IAgendamentoRepository
{
    Task<Agendamento?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Agendamento>> ObterPorClienteIdAsync(Guid clienteId);
    Task<IEnumerable<Agendamento>> ObterPorDataAsync(DateTime data);
    Task<IEnumerable<Agendamento>> ObterTodosAsync();
    Task<bool> ExisteAgendamentoNoHorarioAsync(DateTime dataHora);
    Task<Agendamento> AdicionarAsync(Agendamento agendamento);
    Task<bool> AtualizarAsync(Agendamento agendamento);
    Task<bool> RemoverAsync(Guid id);
    Task<IEnumerable<Agendamento>> PesquisarAsync(
        Guid? clienteId,
        DateTime? dataInicio,
        DateTime? dataFim,
        StatusAgendamento? status,
        string? texto);
}
