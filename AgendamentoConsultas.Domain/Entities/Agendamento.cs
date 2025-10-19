using AgendamentoConsultas.Domain.Common;

namespace AgendamentoConsultas.Domain.Entities;

public class Agendamento : BaseEntity
{
    public Guid ClienteId { get; private set; }
    public Cliente Cliente { get; private set; }
    public DateTime DataHora { get; private set; }
    public StatusAgendamento Status { get; private set; }
    public string? Observacoes { get; private set; }

    private Agendamento() { }

    public Agendamento(Guid clienteId, DateTime dataHora, string? observacoes = null)
    {
        ValidarDataHora(dataHora);
        
        ClienteId = clienteId;
        DataHora = dataHora;
        Status = StatusAgendamento.Agendado;
        Observacoes = observacoes;
    }

    public void Reagendar(DateTime novaDataHora)
    {
        if (Status == StatusAgendamento.Cancelado)
            throw new InvalidOperationException("Não é possível reagendar um agendamento cancelado");
        
        if (Status == StatusAgendamento.Concluido)
            throw new InvalidOperationException("Não é possível reagendar um agendamento concluído");
        
        ValidarDataHora(novaDataHora);
        
        DataHora = novaDataHora;
        AtualizarDataModificacao();
    }

    public void Cancelar()
    {
        if (Status == StatusAgendamento.Cancelado)
            throw new InvalidOperationException("Agendamento já está cancelado");
        
        if (Status == StatusAgendamento.Concluido)
            throw new InvalidOperationException("Não é possível cancelar um agendamento concluído");
        
        Status = StatusAgendamento.Cancelado;
        AtualizarDataModificacao();
    }

    public void Concluir()
    {
        if (Status == StatusAgendamento.Cancelado)
            throw new InvalidOperationException("Não é possível concluir um agendamento cancelado");
        
        if (Status == StatusAgendamento.Concluido)
            throw new InvalidOperationException("Agendamento já está concluído");
        
        Status = StatusAgendamento.Concluido;
        AtualizarDataModificacao();
    }

    public void AtualizarObservacoes(string? observacoes)
    {
        Observacoes = observacoes;
        AtualizarDataModificacao();
    }

    private void ValidarDataHora(DateTime dataHora)
    {
        if (dataHora <= DateTime.Now)
            throw new ArgumentException("Data e hora do agendamento deve ser futura", nameof(dataHora));
        
        if (dataHora.Minute != 0 && dataHora.Minute != 30)
            throw new ArgumentException("Agendamentos devem ser em horários de 30 em 30 minutos", nameof(dataHora));
        
        if (dataHora.Hour < 8 || dataHora.Hour >= 18)
            throw new ArgumentException("Horário de atendimento é das 08:00 às 18:00", nameof(dataHora));
        
        if (dataHora.DayOfWeek == DayOfWeek.Saturday || dataHora.DayOfWeek == DayOfWeek.Sunday)
            throw new ArgumentException("Não há atendimento aos finais de semana", nameof(dataHora));
    }
}

public enum StatusAgendamento
{
    Agendado = 1,
    Concluido = 2,
    Cancelado = 3
}
