using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Domain.Entities;
using AgendamentoConsultas.Infrastructure.Data;

namespace AgendamentoConsultas.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly InMemoryDatabase _database;

    public ClienteRepository()
    {
        _database = InMemoryDatabase.Instance;
    }

    public Task<Cliente?> ObterPorIdAsync(Guid id)
    {
        var cliente = _database.Clientes.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(cliente);
    }

    public Task<Cliente?> ObterPorCpfAsync(string cpf)
    {
        var cpfLimpo = cpf.Replace(".", "").Replace("-", "").Trim();
        var cliente = _database.Clientes.FirstOrDefault(c => 
            c.Cpf.Replace(".", "").Replace("-", "").Trim() == cpfLimpo);
        return Task.FromResult(cliente);
    }

    public Task<IEnumerable<Cliente>> ObterTodosAsync()
    {
        return Task.FromResult<IEnumerable<Cliente>>(_database.Clientes.ToList());
    }

    public Task<Cliente> AdicionarAsync(Cliente cliente)
    {
        _database.Clientes.Add(cliente);
        return Task.FromResult(cliente);
    }

    public Task<bool> AtualizarAsync(Cliente cliente)
    {
        var index = _database.Clientes.FindIndex(c => c.Id == cliente.Id);
        if (index >= 0)
        {
            _database.Clientes[index] = cliente;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> RemoverAsync(Guid id)
    {
        var cliente = _database.Clientes.FirstOrDefault(c => c.Id == id);
        if (cliente != null)
        {
            _database.Clientes.Remove(cliente);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
