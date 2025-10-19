namespace AgendamentoConsultas.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CriadoEm { get; protected set; }
    public DateTime? AtualizadoEm { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CriadoEm = DateTime.UtcNow;
    }

    protected void AtualizarDataModificacao()
    {
        AtualizadoEm = DateTime.UtcNow;
    }
}
