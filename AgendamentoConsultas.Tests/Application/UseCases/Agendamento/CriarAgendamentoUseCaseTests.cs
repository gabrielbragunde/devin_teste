using AgendamentoConsultas.Application.DTOs.Agendamento;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Application.UseCases.Agendamento;
using FluentAssertions;
using Moq;
using Xunit;

namespace AgendamentoConsultas.Tests.Application.UseCases.Agendamento;

public class CriarAgendamentoUseCaseTests
{
    private readonly Mock<IAgendamentoRepository> _agendamentoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly CriarAgendamentoUseCase _useCase;

    public CriarAgendamentoUseCaseTests()
    {
        _agendamentoRepositoryMock = new Mock<IAgendamentoRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _useCase = new CriarAgendamentoUseCase(
            _agendamentoRepositoryMock.Object,
            _clienteRepositoryMock.Object
        );
    }

    [Fact]
    public async Task ExecutarAsync_ComDadosValidos_DeveCriarAgendamento()
    {
        var clienteId = Guid.NewGuid();
        var cliente = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        ).Value;

        var dto = new CriarAgendamentoDto(
            clienteId,
            DateTime.Now.AddDays(1).Date.AddHours(10),
            "Primeira consulta"
        );

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId))
            .ReturnsAsync(cliente);

        _agendamentoRepositoryMock
            .Setup(x => x.ExisteAgendamentoNoHorarioAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(false);

        _agendamentoRepositoryMock
            .Setup(x => x.AdicionarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Agendamento>()))
            .ReturnsAsync((AgendamentoConsultas.Domain.Entities.Agendamento a) => a);

        var result = await _useCase.ExecutarAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value.ClienteId.Should().Be(clienteId);
        result.Value.DataHora.Should().Be(dto.DataHora);
        result.Value.Observacoes.Should().Be(dto.Observacoes);
        result.Value.NomeCliente.Should().Be(cliente.Nome);

        _clienteRepositoryMock.Verify(x => x.ObterPorIdAsync(clienteId), Times.Once);
        _agendamentoRepositoryMock.Verify(x => x.ExisteAgendamentoNoHorarioAsync(dto.DataHora), Times.Once);
        _agendamentoRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Agendamento>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComClienteInexistente_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();
        var dto = new CriarAgendamentoDto(
            clienteId,
            DateTime.Now.AddDays(1).Date.AddHours(10),
            "Primeira consulta"
        );

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId))
            .ReturnsAsync((AgendamentoConsultas.Domain.Entities.Cliente?)null);

        var result = await _useCase.ExecutarAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Cliente não encontrado");

        _clienteRepositoryMock.Verify(x => x.ObterPorIdAsync(clienteId), Times.Once);
        _agendamentoRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Agendamento>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComHorarioOcupado_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();
        var cliente = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        ).Value;

        var dto = new CriarAgendamentoDto(
            clienteId,
            DateTime.Now.AddDays(1).Date.AddHours(10),
            "Primeira consulta"
        );

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId))
            .ReturnsAsync(cliente);

        _agendamentoRepositoryMock
            .Setup(x => x.ExisteAgendamentoNoHorarioAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(true);

        var result = await _useCase.ExecutarAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Já existe um agendamento neste horário");

        _agendamentoRepositoryMock.Verify(x => x.ExisteAgendamentoNoHorarioAsync(dto.DataHora), Times.Once);
        _agendamentoRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Agendamento>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComDataHoraPassada_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();
        var dto = new CriarAgendamentoDto(
            clienteId,
            DateTime.Now.AddDays(-1),
            "Primeira consulta"
        );

        var result = await _useCase.ExecutarAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Data e hora do agendamento deve ser futura");

        _agendamentoRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Agendamento>()), Times.Never);
    }
}
