using AgendamentoConsultas.Domain.Entities;

namespace AgendamentoConsultas.Infrastructure.Data;

public class InMemoryDatabase
{
    private static InMemoryDatabase? _instance;
    private static readonly object _lock = new object();

    public List<Cliente> Clientes { get; private set; }
    public List<Agendamento> Agendamentos { get; private set; }

    private InMemoryDatabase()
    {
        Clientes = new List<Cliente>();
        Agendamentos = new List<Agendamento>();
    }

    public static InMemoryDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new InMemoryDatabase();
                    }
                }
            }
            return _instance;
        }
    }

    public void LimparDados()
    {
        Clientes.Clear();
        Agendamentos.Clear();
    }
}
