using AgendamentoConsultas.Domain.Common;
using AgendamentoConsultas.Domain.Patterns;

namespace AgendamentoConsultas.Domain.Entities;

public class Agendamento : BaseEntity
{
    public Guid ClienteId { get; private set; }
    public Cliente Cliente { get; private set; }
    public DateTime DataHora { get; private set; }
    public StatusAgendamento Status { get; private set; }
    public string? Observacoes { get; private set; }

    private Agendamento() { }

    private Agendamento(Guid clienteId, DateTime dataHora, string? observacoes = null)
    {
        ClienteId = clienteId;
        DataHora = dataHora;
        Status = StatusAgendamento.Agendado;
        Observacoes = observacoes;
    }

    public static Result<Agendamento> Criar(Guid clienteId, DateTime dataHora, string? observacoes = null)
    {
        var notification = new Notification();
        
        ValidarDataHora(dataHora, notification);
        
        if (notification.HasErrors)
            return Result.Failure<Agendamento>(notification.GetErrorsAsString());
        
        var agendamento = new Agendamento(clienteId, dataHora, observacoes);
        return Result.Success(agendamento);
    }

    public Result Reagendar(DateTime novaDataHora)
    {
        var notification = new Notification();
        
        if (Status == StatusAgendamento.Cancelado)
            notification.AddError("Não é possível reagendar um agendamento cancelado");
        
        if (Status == StatusAgendamento.Concluido)
            notification.AddError("Não é possível reagendar um agendamento concluído");
        
        ValidarDataHora(novaDataHora, notification);
        
        if (notification.HasErrors)
            return Result.Failure(notification.GetErrorsAsString());
        
        DataHora = novaDataHora;
        AtualizarDataModificacao();
        
        return Result.Success();
    }

    public Result Cancelar()
    {
        var notification = new Notification();
        
        if (Status == StatusAgendamento.Cancelado)
            notification.AddError("Agendamento já está cancelado");
        
        if (Status == StatusAgendamento.Concluido)
            notification.AddError("Não é possível cancelar um agendamento concluído");
        
        if (notification.HasErrors)
            return Result.Failure(notification.GetErrorsAsString());
        
        Status = StatusAgendamento.Cancelado;
        AtualizarDataModificacao();
        
        return Result.Success();
    }

    public Result Concluir()
    {
        var notification = new Notification();
        
        if (Status == StatusAgendamento.Cancelado)
            notification.AddError("Não é possível concluir um agendamento cancelado");
        
        if (Status == StatusAgendamento.Concluido)
            notification.AddError("Agendamento já está concluído");
        
        if (notification.HasErrors)
            return Result.Failure(notification.GetErrorsAsString());
        
        Status = StatusAgendamento.Concluido;
        AtualizarDataModificacao();
        
        return Result.Success();
    }

    public void AtualizarObservacoes(string? observacoes)
    {
        Observacoes = observacoes;
        AtualizarDataModificacao();
    }

    private static void ValidarDataHora(DateTime dataHora, Notification notification)
    {
        if (dataHora <= DateTime.Now)
            notification.AddError("Data e hora do agendamento deve ser futura");
        
        if (dataHora.Minute != 0 && dataHora.Minute != 30)
            notification.AddError("Agendamentos devem ser em horários de 30 em 30 minutos");
        
        if (dataHora.Hour < 8 || dataHora.Hour >= 18)
            notification.AddError("Horário de atendimento é das 08:00 às 18:00");
        
        if (dataHora.DayOfWeek == DayOfWeek.Saturday || dataHora.DayOfWeek == DayOfWeek.Sunday)
            notification.AddError("Não há atendimento aos finais de semana");
    }
}

public enum StatusAgendamento
{
    Agendado = 1,
    Concluido = 2,
    Cancelado = 3
}
