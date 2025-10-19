namespace AgendamentoConsultas.Application.DTOs.Agendamento;

public record CriarAgendamentoDto(
    Guid ClienteId,
    DateTime DataHora,
    string? Observacoes
);
