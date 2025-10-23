using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Application.UseCases.Agendamento;
using FluentAssertions;
using Moq;
using Xunit;

namespace AgendamentoConsultas.Tests.Application.UseCases.Agendamento;

public class ObterAgendamentosPorClienteUseCaseTests
{
    private readonly Mock<IAgendamentoRepository> _agendamentoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly ObterAgendamentosPorClienteUseCase _useCase;

    public ObterAgendamentosPorClienteUseCaseTests()
    {
        _agendamentoRepositoryMock = new Mock<IAgendamentoRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _useCase = new ObterAgendamentosPorClienteUseCase(
            _agendamentoRepositoryMock.Object,
            _clienteRepositoryMock.Object
        );
    }

    [Fact]
    public async Task ExecutarAsync_ComClienteExistente_DeveRetornarAgendamentos()
    {
        var clienteId = Guid.NewGuid();
        var cliente = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        ).Value;

        var agendamento1 = AgendamentoConsultas.Domain.Entities.Agendamento.Criar(
            clienteId,
            DateTime.Now.AddDays(1).Date.AddHours(10),
            "Primeira consulta"
        ).Value;

        var data2 = DateTime.Now.AddDays(2).Date.AddHours(14);
        if (data2.DayOfWeek == DayOfWeek.Saturday) data2 = data2.AddDays(2);
        if (data2.DayOfWeek == DayOfWeek.Sunday) data2 = data2.AddDays(1);

        var agendamento2 = AgendamentoConsultas.Domain.Entities.Agendamento.Criar(
            clienteId,
            data2,
            "Segunda consulta"
        ).Value;

        var agendamentos = new List<AgendamentoConsultas.Domain.Entities.Agendamento> { agendamento1, agendamento2 };

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId))
            .ReturnsAsync(cliente);

        _agendamentoRepositoryMock
            .Setup(x => x.ObterPorClienteIdAsync(clienteId))
            .ReturnsAsync(agendamentos);

        var result = await _useCase.ExecutarAsync(clienteId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);

        _clienteRepositoryMock.Verify(x => x.ObterPorIdAsync(clienteId), Times.Once);
        _agendamentoRepositoryMock.Verify(x => x.ObterPorClienteIdAsync(clienteId), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComClienteInexistente_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId))
            .ReturnsAsync((AgendamentoConsultas.Domain.Entities.Cliente?)null);

        var result = await _useCase.ExecutarAsync(clienteId);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Cliente não encontrado");

        _clienteRepositoryMock.Verify(x => x.ObterPorIdAsync(clienteId), Times.Once);
        _agendamentoRepositoryMock.Verify(x => x.ObterPorClienteIdAsync(It.IsAny<Guid>()), Times.Never);
    }
}
