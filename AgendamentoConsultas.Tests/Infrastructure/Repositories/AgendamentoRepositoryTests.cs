using AgendamentoConsultas.Domain.Entities;
using AgendamentoConsultas.Infrastructure.Repositories;
using FluentAssertions;
using Xunit;

namespace AgendamentoConsultas.Tests.Infrastructure.Repositories;

public class AgendamentoRepositoryTests
{
    [Fact]
    public async Task AdicionarAsync_DeveAdicionarAgendamento()
    {
        var repository = new AgendamentoRepository();
        var clienteId = Guid.NewGuid();
        var agendamento = Agendamento.Criar(
            clienteId,
            DateTime.Now.AddDays(1).Date.AddHours(10),
            "Primeira consulta"
        ).Value;

        var result = await repository.AdicionarAsync(agendamento);

        result.Should().NotBeNull();
        result.Id.Should().Be(agendamento.Id);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdExistente_DeveRetornarAgendamento()
    {
        var repository = new AgendamentoRepository();
        var clienteId = Guid.NewGuid();
        var agendamento = Agendamento.Criar(
            clienteId,
            DateTime.Now.AddDays(1).Date.AddHours(10),
            "Primeira consulta"
        ).Value;

        await repository.AdicionarAsync(agendamento);
        var result = await repository.ObterPorIdAsync(agendamento.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(agendamento.Id);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInexistente_DeveRetornarNull()
    {
        var repository = new AgendamentoRepository();
        var id = Guid.NewGuid();

        var result = await repository.ObterPorIdAsync(id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ObterPorClienteIdAsync_DeveRetornarAgendamentosDoCliente()
    {
        var repository = new AgendamentoRepository();
        var clienteId = Guid.NewGuid();
        var agendamento1 = Agendamento.Criar(
            clienteId,
            DateTime.Now.AddDays(1).Date.AddHours(10),
            "Primeira consulta"
        ).Value;

        var agendamento2 = Agendamento.Criar(
            clienteId,
            DateTime.Now.AddDays(2).Date.AddHours(14),
            "Segunda consulta"
        ).Value;

        await repository.AdicionarAsync(agendamento1);
        await repository.AdicionarAsync(agendamento2);

        var result = await repository.ObterPorClienteIdAsync(clienteId);

        result.Should().HaveCount(c => c >= 2);
    }

    [Fact]
    public async Task ObterPorDataAsync_DeveRetornarAgendamentosDaData()
    {
        var repository = new AgendamentoRepository();
        var clienteId = Guid.NewGuid();
        var data = DateTime.Now.AddDays(1).Date;
        var agendamento = Agendamento.Criar(
            clienteId,
            data.AddHours(10),
            "Primeira consulta"
        ).Value;

        await repository.AdicionarAsync(agendamento);

        var result = await repository.ObterPorDataAsync(data);

        result.Should().HaveCount(c => c >= 1);
    }

    [Fact]
    public async Task ExisteAgendamentoNoHorarioAsync_ComHorarioOcupado_DeveRetornarTrue()
    {
        var repository = new AgendamentoRepository();
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1).Date.AddHours(10);
        var agendamento = Agendamento.Criar(
            clienteId,
            dataHora,
            "Primeira consulta"
        ).Value;

        await repository.AdicionarAsync(agendamento);

        var result = await repository.ExisteAgendamentoNoHorarioAsync(dataHora);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExisteAgendamentoNoHorarioAsync_ComHorarioLivre_DeveRetornarFalse()
    {
        var repository = new AgendamentoRepository();
        var dataHora = DateTime.Now.AddDays(10).Date.AddHours(10);

        var result = await repository.ExisteAgendamentoNoHorarioAsync(dataHora);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task AtualizarAsync_ComAgendamentoExistente_DeveAtualizarERetornarTrue()
    {
        var repository = new AgendamentoRepository();
        var clienteId = Guid.NewGuid();
        var agendamento = Agendamento.Criar(
            clienteId,
            DateTime.Now.AddDays(1).Date.AddHours(10),
            "Primeira consulta"
        ).Value;

        await repository.AdicionarAsync(agendamento);
        agendamento.AtualizarObservacoes("Consulta atualizada");

        var result = await repository.AtualizarAsync(agendamento);

        result.Should().BeTrue();

        var agendamentoAtualizado = await repository.ObterPorIdAsync(agendamento.Id);
        agendamentoAtualizado!.Observacoes.Should().Be("Consulta atualizada");
    }

    [Fact]
    public async Task AtualizarAsync_ComAgendamentoInexistente_DeveRetornarFalse()
    {
        var repository = new AgendamentoRepository();
        var clienteId = Guid.NewGuid();
        var agendamento = Agendamento.Criar(
            clienteId,
            DateTime.Now.AddDays(1).Date.AddHours(10),
            "Primeira consulta"
        ).Value;

        var result = await repository.AtualizarAsync(agendamento);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task RemoverAsync_ComAgendamentoExistente_DeveRemoverERetornarTrue()
    {
        var repository = new AgendamentoRepository();
        var clienteId = Guid.NewGuid();
        var agendamento = Agendamento.Criar(
            clienteId,
            DateTime.Now.AddDays(1).Date.AddHours(10),
            "Primeira consulta"
        ).Value;

        await repository.AdicionarAsync(agendamento);
        var result = await repository.RemoverAsync(agendamento.Id);

        result.Should().BeTrue();

        var agendamentoRemovido = await repository.ObterPorIdAsync(agendamento.Id);
        agendamentoRemovido.Should().BeNull();
    }

    [Fact]
    public async Task RemoverAsync_ComAgendamentoInexistente_DeveRetornarFalse()
    {
        var repository = new AgendamentoRepository();
        var id = Guid.NewGuid();

        var result = await repository.RemoverAsync(id);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarTodosAgendamentos()
    {
        var repository = new AgendamentoRepository();
        var clienteId = Guid.NewGuid();
        var agendamento1 = Agendamento.Criar(
            clienteId,
            DateTime.Now.AddDays(1).Date.AddHours(10),
            "Primeira consulta"
        ).Value;

        var agendamento2 = Agendamento.Criar(
            clienteId,
            DateTime.Now.AddDays(2).Date.AddHours(14),
            "Segunda consulta"
        ).Value;

        await repository.AdicionarAsync(agendamento1);
        await repository.AdicionarAsync(agendamento2);

        var result = await repository.ObterTodosAsync();

        result.Should().HaveCount(c => c >= 2);
    }
}
