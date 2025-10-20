using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Application.UseCases.Agendamento;
using AgendamentoConsultas.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace AgendamentoConsultas.Tests.Application.UseCases.Agendamento;

public class ObterHorariosDisponiveisUseCaseTests
{
    private readonly Mock<IAgendamentoRepository> _agendamentoRepositoryMock;
    private readonly ObterHorariosDisponiveisUseCase _useCase;

    public ObterHorariosDisponiveisUseCaseTests()
    {
        _agendamentoRepositoryMock = new Mock<IAgendamentoRepository>();
        _useCase = new ObterHorariosDisponiveisUseCase(_agendamentoRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecutarAsync_ComDataValida_DeveRetornarHorariosDisponiveis()
    {
        var data = GetNextWeekday().Date;
        var agendamentos = new List<AgendamentoConsultas.Domain.Entities.Agendamento>();

        _agendamentoRepositoryMock
            .Setup(x => x.ObterPorDataAsync(data))
            .ReturnsAsync(agendamentos);

        var result = await _useCase.ExecutarAsync(data);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _agendamentoRepositoryMock.Verify(x => x.ObterPorDataAsync(data), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComDataPassada_DeveRetornarFalha()
    {
        var data = DateTime.Today.AddDays(-1);

        var result = await _useCase.ExecutarAsync(data);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Não é possível consultar horários de datas passadas");

        _agendamentoRepositoryMock.Verify(x => x.ObterPorDataAsync(It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_NoSabado_DeveRetornarListaVazia()
    {
        var data = GetNextSaturday();

        var result = await _useCase.ExecutarAsync(data);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();

        _agendamentoRepositoryMock.Verify(x => x.ObterPorDataAsync(It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_NoDomingo_DeveRetornarListaVazia()
    {
        var data = GetNextSunday();

        var result = await _useCase.ExecutarAsync(data);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();

        _agendamentoRepositoryMock.Verify(x => x.ObterPorDataAsync(It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComHorariosOcupados_DeveMarcaComoIndisponivel()
    {
        var data = GetNextWeekday().Date;
        var clienteId = Guid.NewGuid();
        var dataHora = data.AddHours(10);
        
        var agendamento = AgendamentoConsultas.Domain.Entities.Agendamento.Criar(
            clienteId,
            dataHora,
            "Consulta"
        ).Value;

        var agendamentos = new List<AgendamentoConsultas.Domain.Entities.Agendamento> { agendamento };

        _agendamentoRepositoryMock
            .Setup(x => x.ObterPorDataAsync(data))
            .ReturnsAsync(agendamentos);

        var result = await _useCase.ExecutarAsync(data);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        _agendamentoRepositoryMock.Verify(x => x.ObterPorDataAsync(data), Times.Once);
    }

    private DateTime GetNextWeekday()
    {
        var date = DateTime.Now.AddDays(1);
        while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            date = date.AddDays(1);
        }
        return date;
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
