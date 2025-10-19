using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Domain.Entities;

namespace AgendamentoConsultas.Application.Extensions;

public static class AgendamentoExtensions
{
    public static AgendamentoDto ToDto(this Agendamento agendamento, string nomeCliente)
    {
        return new AgendamentoDto(
            agendamento.Id,
            agendamento.ClienteId,
            nomeCliente,
            agendamento.DataHora,
            agendamento.Status.ToString(),
            agendamento.Observacoes,
            agendamento.CriadoEm,
            agendamento.AtualizadoEm
        );
    }
}
