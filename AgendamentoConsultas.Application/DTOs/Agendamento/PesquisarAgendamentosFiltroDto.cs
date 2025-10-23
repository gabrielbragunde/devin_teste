namespace AgendamentoConsultas.Application.DTOs.Agendamento;

public record PesquisarAgendamentosFiltroDto(
    Guid? ClienteId,
    DateTime? DataInicio,
    DateTime? DataFim,
    string? Status,
    string? Texto
);
