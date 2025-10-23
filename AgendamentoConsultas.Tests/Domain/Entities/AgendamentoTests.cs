using AgendamentoConsultas.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AgendamentoConsultas.Tests.Domain.Entities;

public class AgendamentoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarAgendamento()
    {
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1).Date.AddHours(10);
        var observacoes = "Primeira consulta";

        var result = Agendamento.Criar(clienteId, dataHora, observacoes);

        result.IsSuccess.Should().BeTrue();
        result.Value.ClienteId.Should().Be(clienteId);
        result.Value.DataHora.Should().Be(dataHora);
        result.Value.Observacoes.Should().Be(observacoes);
        result.Value.Status.Should().Be(StatusAgendamento.Agendado);
    }

    [Fact]
    public void Criar_ComDataHoraPassada_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(-1);
        var observacoes = "Primeira consulta";

        var result = Agendamento.Criar(clienteId, dataHora, observacoes);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Data e hora do agendamento deve ser futura");
    }

    [Theory]
    [InlineData(15)]
    [InlineData(45)]
    [InlineData(5)]
    [InlineData(25)]
    public void Criar_ComMinutosInvalidos_DeveRetornarFalha(int minutos)
    {
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1).Date.AddHours(10).AddMinutes(minutos);
        var observacoes = "Primeira consulta";

        var result = Agendamento.Criar(clienteId, dataHora, observacoes);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Agendamentos devem ser em horários de 30 em 30 minutos");
    }

    [Theory]
    [InlineData(7)]
    [InlineData(18)]
    [InlineData(19)]
    [InlineData(23)]
    public void Criar_ComHorarioForaDoAtendimento_DeveRetornarFalha(int hora)
    {
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1).Date.AddHours(hora);
        var observacoes = "Primeira consulta";

        var result = Agendamento.Criar(clienteId, dataHora, observacoes);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Horário de atendimento é das 08:00 às 18:00");
    }

    [Fact]
    public void Criar_NoSabado_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();
        var dataHora = GetNextSaturday().AddHours(10);
        var observacoes = "Primeira consulta";

        var result = Agendamento.Criar(clienteId, dataHora, observacoes);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Não há atendimento aos finais de semana");
    }

    [Fact]
    public void Criar_NoDomingo_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();
        var dataHora = GetNextSunday().AddHours(10);
        var observacoes = "Primeira consulta";

        var result = Agendamento.Criar(clienteId, dataHora, observacoes);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Não há atendimento aos finais de semana");
    }

    [Fact]
    public void Reagendar_ComDataHoraValida_DeveReagendar()
    {
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1).Date.AddHours(10);
        var agendamento = Agendamento.Criar(clienteId, dataHora, "Primeira consulta").Value;
        var novaDataHora = DateTime.Now.AddDays(2).Date.AddHours(14);
        while (novaDataHora.DayOfWeek == DayOfWeek.Saturday || novaDataHora.DayOfWeek == DayOfWeek.Sunday)
        {
            novaDataHora = novaDataHora.AddDays(1);
        }

        var result = agendamento.Reagendar(novaDataHora);

        result.IsSuccess.Should().BeTrue();
        agendamento.DataHora.Should().Be(novaDataHora);
        agendamento.AtualizadoEm.Should().NotBeNull();
    }

    [Fact]
    public void Reagendar_AgendamentoCancelado_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1).Date.AddHours(10);
        var agendamento = Agendamento.Criar(clienteId, dataHora, "Primeira consulta").Value;
        agendamento.Cancelar();
        var novaDataHora = DateTime.Now.AddDays(2).Date.AddHours(14);

        var result = agendamento.Reagendar(novaDataHora);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Não é possível reagendar um agendamento cancelado");
    }

    [Fact]
    public void Reagendar_AgendamentoConcluido_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1).Date.AddHours(10);
        var agendamento = Agendamento.Criar(clienteId, dataHora, "Primeira consulta").Value;
        agendamento.Concluir();
        var novaDataHora = DateTime.Now.AddDays(2).Date.AddHours(14);

        var result = agendamento.Reagendar(novaDataHora);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Não é possível reagendar um agendamento concluído");
    }

    [Fact]
    public void Cancelar_AgendamentoValido_DeveCancelar()
    {
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1).Date.AddHours(10);
        var agendamento = Agendamento.Criar(clienteId, dataHora, "Primeira consulta").Value;

        var result = agendamento.Cancelar();

        result.IsSuccess.Should().BeTrue();
        agendamento.Status.Should().Be(StatusAgendamento.Cancelado);
        agendamento.AtualizadoEm.Should().NotBeNull();
    }

    [Fact]
    public void Cancelar_AgendamentoJaCancelado_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1).Date.AddHours(10);
        var agendamento = Agendamento.Criar(clienteId, dataHora, "Primeira consulta").Value;
        agendamento.Cancelar();

        var result = agendamento.Cancelar();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Agendamento já está cancelado");
    }

    [Fact]
    public void Cancelar_AgendamentoConcluido_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1).Date.AddHours(10);
        var agendamento = Agendamento.Criar(clienteId, dataHora, "Primeira consulta").Value;
        agendamento.Concluir();

        var result = agendamento.Cancelar();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Não é possível cancelar um agendamento concluído");
    }

    [Fact]
    public void Concluir_AgendamentoValido_DeveConcluir()
    {
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1).Date.AddHours(10);
        var agendamento = Agendamento.Criar(clienteId, dataHora, "Primeira consulta").Value;

        var result = agendamento.Concluir();

        result.IsSuccess.Should().BeTrue();
        agendamento.Status.Should().Be(StatusAgendamento.Concluido);
        agendamento.AtualizadoEm.Should().NotBeNull();
    }

    [Fact]
    public void Concluir_AgendamentoCancelado_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1).Date.AddHours(10);
        var agendamento = Agendamento.Criar(clienteId, dataHora, "Primeira consulta").Value;
        agendamento.Cancelar();

        var result = agendamento.Concluir();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Não é possível concluir um agendamento cancelado");
    }

    [Fact]
    public void Concluir_AgendamentoJaConcluido_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1).Date.AddHours(10);
        var agendamento = Agendamento.Criar(clienteId, dataHora, "Primeira consulta").Value;
        agendamento.Concluir();

        var result = agendamento.Concluir();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Agendamento já está concluído");
    }

    [Fact]
    public void AtualizarObservacoes_DeveAtualizarObservacoes()
    {
        var clienteId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1).Date.AddHours(10);
        var agendamento = Agendamento.Criar(clienteId, dataHora, "Primeira consulta").Value;
        var novasObservacoes = "Consulta de retorno";

        agendamento.AtualizarObservacoes(novasObservacoes);

        agendamento.Observacoes.Should().Be(novasObservacoes);
        agendamento.AtualizadoEm.Should().NotBeNull();
    }

    private DateTime GetNextSaturday()
    {
        var date = DateTime.Now.AddDays(1);
        while (date.DayOfWeek != DayOfWeek.Saturday)
        {
            date = date.AddDays(1);
        }
        return date.Date;
    }

    private DateTime GetNextSunday()
    {
        var date = DateTime.Now.AddDays(1);
        while (date.DayOfWeek != DayOfWeek.Sunday)
        {
            date = date.AddDays(1);
        }
        return date.Date;
    }
}
