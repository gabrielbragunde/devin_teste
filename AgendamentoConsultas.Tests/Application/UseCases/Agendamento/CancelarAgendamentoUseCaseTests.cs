using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Application.UseCases.Agendamento;
using FluentAssertions;
using Moq;
using Xunit;

namespace AgendamentoConsultas.Tests.Application.UseCases.Agendamento;

public class CancelarAgendamentoUseCaseTests
{
    private readonly Mock<IAgendamentoRepository> _agendamentoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly CancelarAgendamentoUseCase _useCase;

    public CancelarAgendamentoUseCaseTests()
    {
        _agendamentoRepositoryMock = new Mock<IAgendamentoRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _useCase = new CancelarAgendamentoUseCase(
            _agendamentoRepositoryMock.Object,
            _clienteRepositoryMock.Object
        );
    }

    [Fact]
    public async Task ExecutarAsync_ComAgendamentoValido_DeveCancelar()
    {
        var agendamentoId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var cliente = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        ).Value;

        var agendamento = AgendamentoConsultas.Domain.Entities.Agendamento.Criar(
            clienteId,
            DateTime.Now.AddDays(1).Date.AddHours(10),
            "Primeira consulta"
        ).Value;

        _agendamentoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(agendamentoId))
            .ReturnsAsync(agendamento);

        _agendamentoRepositoryMock
            .Setup(x => x.AtualizarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Agendamento>()))
            .ReturnsAsync(true);

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId))
            .ReturnsAsync(cliente);

        var result = await _useCase.ExecutarAsync(agendamentoId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Cancelado");

        _agendamentoRepositoryMock.Verify(x => x.ObterPorIdAsync(agendamentoId), Times.Once);
        _agendamentoRepositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Agendamento>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComAgendamentoInexistente_DeveRetornarFalha()
    {
        var agendamentoId = Guid.NewGuid();

        _agendamentoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(agendamentoId))
            .ReturnsAsync((AgendamentoConsultas.Domain.Entities.Agendamento?)null);

        var result = await _useCase.ExecutarAsync(agendamentoId);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Agendamento não encontrado");

        _agendamentoRepositoryMock.Verify(x => x.ObterPorIdAsync(agendamentoId), Times.Once);
        _agendamentoRepositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Agendamento>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComFalhaNoRepositorio_DeveRetornarFalha()
    {
        var agendamentoId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var agendamento = AgendamentoConsultas.Domain.Entities.Agendamento.Criar(
            clienteId,
            DateTime.Now.AddDays(1).Date.AddHours(10),
            "Primeira consulta"
        ).Value;

        _agendamentoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(agendamentoId))
            .ReturnsAsync(agendamento);

        _agendamentoRepositoryMock
            .Setup(x => x.AtualizarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Agendamento>()))
            .ReturnsAsync(false);

        var result = await _useCase.ExecutarAsync(agendamentoId);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Falha ao atualizar agendamento no repositório");

        _agendamentoRepositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Agendamento>()), Times.Once);
    }
}
