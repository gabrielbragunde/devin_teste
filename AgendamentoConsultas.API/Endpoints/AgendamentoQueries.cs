using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.UseCases.Agendamento;
using HotChocolate;
using HotChocolate.Execution;

namespace AgendamentoConsultas.API.Endpoints;

public class AgendamentoQueries
{
    public async Task<IEnumerable<AgendamentoDto>> PesquisarAgendamentos(
        [Service] PesquisarAgendamentosUseCase useCase,
        Guid? clienteId,
        DateTime? dataInicio,
        DateTime? dataFim,
        string? status,
        string? texto)
    {
        var filtro = new PesquisarAgendamentosFiltroDto(
            clienteId,
            dataInicio,
            dataFim,
            status,
            texto);

        var resultado = await useCase.ExecutarAsync(filtro);
        if (resultado.IsFailure)
        {
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage(resultado.Error)
                .Build());
        }

        return resultado.Value;
    }
}
