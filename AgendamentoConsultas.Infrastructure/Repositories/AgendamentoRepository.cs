using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Domain.Entities;
using AgendamentoConsultas.Infrastructure.Data;

namespace AgendamentoConsultas.Infrastructure.Repositories;

public class AgendamentoRepository : IAgendamentoRepository
{
    private readonly InMemoryDatabase _database;

    public AgendamentoRepository()
    {
        _database = InMemoryDatabase.Instance;
    }

    public Task<Agendamento?> ObterPorIdAsync(Guid id)
    {
        var agendamento = _database.Agendamentos.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(agendamento);
    }

    public Task<IEnumerable<Agendamento>> ObterPorClienteIdAsync(Guid clienteId)
    {
        var agendamentos = _database.Agendamentos
            .Where(a => a != null && a.ClienteId == clienteId)
            .OrderBy(a => a.DataHora)
            .ToList();
        return Task.FromResult<IEnumerable<Agendamento>>(agendamentos);
    }

    public Task<IEnumerable<Agendamento>> ObterPorDataAsync(DateTime data)
    {
        var agendamentos = _database.Agendamentos
            .Where(a => a.DataHora.Date == data.Date)
            .OrderBy(a => a.DataHora)
            .ToList();
        return Task.FromResult<IEnumerable<Agendamento>>(agendamentos);
    }

    public Task<IEnumerable<Agendamento>> ObterTodosAsync()
    {
        return Task.FromResult<IEnumerable<Agendamento>>(_database.Agendamentos.ToList());
    }

    public Task<bool> ExisteAgendamentoNoHorarioAsync(DateTime dataHora)
    {
        var existe = _database.Agendamentos.Any(a => 
            a.DataHora == dataHora && 
            a.Status == StatusAgendamento.Agendado);
        return Task.FromResult(existe);
    }

    public Task<Agendamento> AdicionarAsync(Agendamento agendamento)
    {
        _database.Agendamentos.Add(agendamento);
        return Task.FromResult(agendamento);
    }

    public Task<bool> AtualizarAsync(Agendamento agendamento)
    {
        var index = _database.Agendamentos.FindIndex(a => a.Id == agendamento.Id);
        if (index >= 0)
        {
            _database.Agendamentos[index] = agendamento;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> RemoverAsync(Guid id)
    {
        var agendamento = _database.Agendamentos.FirstOrDefault(a => a.Id == id);
        if (agendamento != null)
        {
            _database.Agendamentos.Remove(agendamento);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<IEnumerable<Agendamento>> PesquisarAsync(
        Guid? clienteId,
        DateTime? dataInicio,
        DateTime? dataFim,
        StatusAgendamento? status,
        string? texto)
    {
        IEnumerable<Agendamento> query = _database.Agendamentos;

        if (clienteId.HasValue)
        {
            query = query.Where(a => a.ClienteId == clienteId.Value);
        }

        if (dataInicio.HasValue)
        {
            query = query.Where(a => a.DataHora >= dataInicio.Value);
        }

        if (dataFim.HasValue)
        {
            query = query.Where(a => a.DataHora <= dataFim.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var termo = texto.Trim();
            query = query.Where(a =>
                (!string.IsNullOrEmpty(a.Observacoes) && a.Observacoes!.Contains(termo, StringComparison.OrdinalIgnoreCase))
            );
        }

        var resultado = query.OrderBy(a => a.DataHora).ToList();
        return Task.FromResult<IEnumerable<Agendamento>>(resultado);
    }
}
