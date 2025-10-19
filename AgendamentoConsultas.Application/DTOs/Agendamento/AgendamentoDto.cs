namespace AgendamentoConsultas.Application.DTOs.Agendamento;

public record AgendamentoDto(
    Guid Id,
    Guid ClienteId,
    string NomeCliente,
    DateTime DataHora,
    string Status,
    string? Observacoes,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
