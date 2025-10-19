using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Application.UseCases.Cliente;

namespace AgendamentoConsultas.API.Endpoints;

public static class ClienteEndpoints
{
    public static void MapClienteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clientes")
            .WithTags("Clientes")
            .WithOpenApi();

        group.MapPost("/", async (
            CriarClienteDto dto,
            CriarClienteUseCase useCase) =>
        {
            var resultado = await useCase.ExecutarAsync(dto);
            
            if (resultado.IsFailure)
                return Results.BadRequest(new { erro = resultado.Error });
            
            return Results.Created($"/api/clientes/{resultado.Value.Id}", resultado.Value);
        })
        .WithName("CriarCliente")
        .WithSummary("Criar um novo cliente")
        .Produces<ClienteDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/cpf/{cpf}", async (
            string cpf,
            ObterClientePorCpfUseCase useCase) =>
        {
            var resultado = await useCase.ExecutarAsync(cpf);
            
            if (resultado.IsFailure)
                return Results.NotFound(new { erro = resultado.Error });
            
            return Results.Ok(resultado.Value);
        })
        .WithName("ObterClientePorCpf")
        .WithSummary("Obter cliente por CPF")
        .Produces<ClienteDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", async (
            Guid id,
            AtualizarClienteDto dto,
            AtualizarClienteUseCase useCase) =>
        {
            var resultado = await useCase.ExecutarAsync(id, dto);
            
            if (resultado.IsFailure)
                return Results.BadRequest(new { erro = resultado.Error });
            
            return Results.Ok(resultado.Value);
        })
        .WithName("AtualizarCliente")
        .WithSummary("Atualizar dados do cliente")
        .Produces<ClienteDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
