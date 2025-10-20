using AgendamentoConsultas.Domain.Entities;

namespace AgendamentoConsultas.Application.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> ObterPorIdAsync(Guid id);
    Task<Cliente?> ObterPorCpfAsync(string cpf);
    Task<IEnumerable<Cliente>> ObterTodosAsync();
    Task<Cliente> AdicionarAsync(Cliente cliente);
    Task<bool> AtualizarAsync(Cliente cliente);
    Task<bool> RemoverAsync(Guid id);
}
