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
            var resultado = await useCase.ExecutarAsync(dto);
            
            if (resultado.IsFailure)
                return Results.BadRequest(new { erro = resultado.Error });
            
            return Results.Created($"/api/agendamentos/{resultado.Value.Id}", resultado.Value);
        })
        .WithName("CriarAgendamento")
        .WithSummary("Criar um novo agendamento")
        .Produces<AgendamentoDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/cliente/{clienteId:guid}", async (
            Guid clienteId,
            ObterAgendamentosPorClienteUseCase useCase) =>
        {
            var resultado = await useCase.ExecutarAsync(clienteId);
            
            if (resultado.IsFailure)
                return Results.NotFound(new { erro = resultado.Error });
            
            return Results.Ok(resultado.Value);
        })
        .WithName("ObterAgendamentosPorCliente")
        .WithSummary("Obter todos os agendamentos de um cliente")
        .Produces<IEnumerable<AgendamentoDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/horarios-disponiveis", async (
            DateTime data,
            ObterHorariosDisponiveisUseCase useCase) =>
        {
            var resultado = await useCase.ExecutarAsync(data);
            
            if (resultado.IsFailure)
                return Results.BadRequest(new { erro = resultado.Error });
            
            return Results.Ok(resultado.Value);
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
            var resultado = await useCase.ExecutarAsync(id, dto);
            
            if (resultado.IsFailure)
                return Results.BadRequest(new { erro = resultado.Error });
            
            return Results.Ok(resultado.Value);
        })
        .WithName("ReagendarAgendamento")
        .WithSummary("Reagendar um agendamento existente")
        .Produces<AgendamentoDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}/cancelar", async (
            Guid id,
            CancelarAgendamentoUseCase useCase) =>
        {
            var resultado = await useCase.ExecutarAsync(id);
            
            if (resultado.IsFailure)
                return Results.BadRequest(new { erro = resultado.Error });
            
            return Results.Ok(resultado.Value);
        })
        .WithName("CancelarAgendamento")
        .WithSummary("Cancelar um agendamento")
        .Produces<AgendamentoDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
