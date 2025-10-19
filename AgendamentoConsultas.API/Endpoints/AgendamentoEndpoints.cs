using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.UseCases.Agendamento;

namespace AgendamentoConsultas.API.Endpoints;

public static class AgendamentoEndpoints
{
    public static void MapAgendamentoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/agendamentos")
            .WithTags("Agendamentos")
            .WithOpenApi();

        group.MapPost("/", async (
            CriarAgendamentoDto dto,
            CriarAgendamentoUseCase useCase) =>
        {
            try
            {
                var resultado = await useCase.ExecutarAsync(dto);
                return Results.Created($"/api/agendamentos/{resultado.Id}", resultado);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { erro = ex.Message });
            }
        })
        .WithName("CriarAgendamento")
        .WithSummary("Criar um novo agendamento")
        .Produces<AgendamentoDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status409Conflict);

        group.MapGet("/cliente/{clienteId:guid}", async (
            Guid clienteId,
            ObterAgendamentosPorClienteUseCase useCase) =>
        {
            try
            {
                var resultado = await useCase.ExecutarAsync(clienteId);
                return Results.Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return Results.NotFound(new { erro = ex.Message });
            }
        })
        .WithName("ObterAgendamentosPorCliente")
        .WithSummary("Obter todos os agendamentos de um cliente")
        .Produces<IEnumerable<AgendamentoDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/horarios-disponiveis", async (
            DateTime data,
            ObterHorariosDisponiveisUseCase useCase) =>
        {
            try
            {
                var resultado = await useCase.ExecutarAsync(data);
                return Results.Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { erro = ex.Message });
            }
        })
        .WithName("ObterHorariosDisponiveis")
        .WithSummary("Obter horários disponíveis para uma data")
        .Produces<IEnumerable<HorarioDisponivelDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}/reagendar", async (
            Guid id,
            ReagendarDto dto,
            ReagendarUseCase useCase) =>
        {
            try
            {
                var resultado = await useCase.ExecutarAsync(id, dto);
                return Results.Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.NotFound(new { erro = ex.Message });
            }
        })
        .WithName("ReagendarAgendamento")
        .WithSummary("Reagendar um agendamento existente")
        .Produces<AgendamentoDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}/cancelar", async (
            Guid id,
            CancelarAgendamentoUseCase useCase) =>
        {
            try
            {
                var resultado = await useCase.ExecutarAsync(id);
                return Results.Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return Results.NotFound(new { erro = ex.Message });
            }
        })
        .WithName("CancelarAgendamento")
        .WithSummary("Cancelar um agendamento")
        .Produces<AgendamentoDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
