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
            try
            {
                var resultado = await useCase.ExecutarAsync(dto);
                return Results.Created($"/api/clientes/{resultado.Id}", resultado);
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
        .WithName("CriarCliente")
        .WithSummary("Criar um novo cliente")
        .Produces<ClienteDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status409Conflict);

        group.MapGet("/cpf/{cpf}", async (
            string cpf,
            ObterClientePorCpfUseCase useCase) =>
        {
            var resultado = await useCase.ExecutarAsync(cpf);
            return resultado != null 
                ? Results.Ok(resultado) 
                : Results.NotFound(new { erro = "Cliente não encontrado" });
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
        .WithName("AtualizarCliente")
        .WithSummary("Atualizar dados do cliente")
        .Produces<ClienteDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
    }
}
