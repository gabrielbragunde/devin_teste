namespace AgendamentoConsultas.Application.DTOs.Agendamento;

public record HorarioDisponivelDto(
    DateTime DataHora,
    bool Disponivel
);
