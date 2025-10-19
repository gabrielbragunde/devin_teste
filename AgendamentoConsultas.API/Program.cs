using AgendamentoConsultas.API.Endpoints;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Application.UseCases.Agendamento;
using AgendamentoConsultas.Application.UseCases.Cliente;
using AgendamentoConsultas.Infrastructure.Repositories;

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

builder.Services.AddScoped<CriarClienteUseCase>();
builder.Services.AddScoped<ObterClientePorCpfUseCase>();
builder.Services.AddScoped<AtualizarClienteUseCase>();

builder.Services.AddScoped<CriarAgendamentoUseCase>();
builder.Services.AddScoped<ObterAgendamentosPorClienteUseCase>();
builder.Services.AddScoped<ObterHorariosDisponiveisUseCase>();
builder.Services.AddScoped<ReagendarUseCase>();
builder.Services.AddScoped<CancelarAgendamentoUseCase>();

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

app.Run();
