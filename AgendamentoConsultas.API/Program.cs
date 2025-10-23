using AgendamentoConsultas.API.Endpoints;
using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Application.UseCases.Agendamento;
using AgendamentoConsultas.Application.UseCases.Cliente;
using AgendamentoConsultas.Application.Validators.Cliente;
using AgendamentoConsultas.Infrastructure.Repositories;
using FluentValidation;
using Scrutor;
using HotChocolate.AspNetCore;
using HotChocolate;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Agendamento de Consultas API", 
        Version = "v1",
        Description = "API para gerenciamento de agendamentos de consultas médicas"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSingleton<IClienteRepository, ClienteRepository>();
builder.Services.AddSingleton<IAgendamentoRepository, AgendamentoRepository>();

// Validators
builder.Services.AddScoped<IValidator<CriarClienteDto>, CriarClienteDtoValidator>();
builder.Services.AddScoped<IValidator<AtualizarClienteDto>, AtualizarClienteDtoValidator>();

// Cliente use cases with decorators
builder.Services.AddScoped<ICriarClienteUseCase, CriarClienteUseCase>();
builder.Services.AddScoped<IObterClientePorCpfUseCase, ObterClientePorCpfUseCase>();
builder.Services.AddScoped<IAtualizarClienteUseCase, AtualizarClienteUseCase>();

builder.Services.Decorate<ICriarClienteUseCase, CriarClienteValidationDecorator>();
builder.Services.Decorate<IAtualizarClienteUseCase, AtualizarClienteValidationDecorator>();

builder.Services.AddScoped<CriarAgendamentoUseCase>();
builder.Services.AddScoped<ObterAgendamentosPorClienteUseCase>();
builder.Services.AddScoped<ObterHorariosDisponiveisUseCase>();
builder.Services.AddScoped<ReagendarUseCase>();
builder.Services.AddScoped<CancelarAgendamentoUseCase>();
builder.Services.AddScoped<PesquisarAgendamentosUseCase>();

// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<AgendamentoQueries>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok(new 
{ 
    mensagem = "API de Agendamento de Consultas",
    versao = "1.0",
    documentacao = "/swagger"
}))
.WithName("Root")
.WithTags("Info")
.ExcludeFromDescription();

app.MapClienteEndpoints();
app.MapAgendamentoEndpoints();

// GraphQL endpoint
app.MapGraphQL("/graphql");

app.Run();
