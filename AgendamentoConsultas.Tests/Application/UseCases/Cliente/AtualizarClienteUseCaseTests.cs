using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Application.UseCases.Cliente;
using FluentAssertions;
using Moq;
using Xunit;

namespace AgendamentoConsultas.Tests.Application.UseCases.Cliente;

public class AtualizarClienteUseCaseTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly AtualizarClienteUseCase _useCase;

    public AtualizarClienteUseCaseTests()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _useCase = new AtualizarClienteUseCase(_clienteRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecutarAsync_ComDadosValidos_DeveAtualizarCliente()
    {
        var clienteId = Guid.NewGuid();
        var cliente = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        ).Value;

        var dto = new AtualizarClienteDto(
            "João Silva Santos",
            "joao.santos@email.com",
            "(11) 99999-8888",
            new DateTime(1990, 5, 15)
        );

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId))
            .ReturnsAsync(cliente);

        _clienteRepositoryMock
            .Setup(x => x.AtualizarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Cliente>()))
            .ReturnsAsync(true);

        var result = await _useCase.ExecutarAsync(clienteId, dto);

        result.IsSuccess.Should().BeTrue();
        result.Value.Nome.Should().Be(dto.Nome);
        result.Value.Email.Should().Be(dto.Email);
        result.Value.Telefone.Should().Be(dto.Telefone);

        _clienteRepositoryMock.Verify(x => x.ObterPorIdAsync(clienteId), Times.Once);
        _clienteRepositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Cliente>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComClienteInexistente_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();
        var dto = new AtualizarClienteDto(
            "João Silva Santos",
            "joao.santos@email.com",
            "(11) 99999-8888",
            new DateTime(1990, 5, 15)
        );

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId))
            .ReturnsAsync((AgendamentoConsultas.Domain.Entities.Cliente?)null);

        var result = await _useCase.ExecutarAsync(clienteId, dto);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Cliente não encontrado");

        _clienteRepositoryMock.Verify(x => x.ObterPorIdAsync(clienteId), Times.Once);
        _clienteRepositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Cliente>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComFalhaNoRepositorio_DeveRetornarFalha()
    {
        var clienteId = Guid.NewGuid();
        var cliente = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        ).Value;

        var dto = new AtualizarClienteDto(
            "João Silva Santos",
            "joao.santos@email.com",
            "(11) 99999-8888",
            new DateTime(1990, 5, 15)
        );

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(clienteId))
            .ReturnsAsync(cliente);

        _clienteRepositoryMock
            .Setup(x => x.AtualizarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Cliente>()))
            .ReturnsAsync(false);

        var result = await _useCase.ExecutarAsync(clienteId, dto);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Falha ao atualizar cliente no repositório");

        _clienteRepositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Cliente>()), Times.Once);
    }

    [Theory]
    [InlineData("", "joao@email.com", "(11) 98765-4321")]
    [InlineData("João", "", "(11) 98765-4321")]
    [InlineData("João", "joao@email.com", "")]
    public async Task ExecutarAsync_ComDadosInvalidos_DeveRetornarFalha(string nome, string email, string telefone)
    {
        var clienteId = Guid.NewGuid();
        var dto = new AtualizarClienteDto(
            nome,
            email,
            telefone,
            new DateTime(1990, 5, 15)
        );

        var result = await _useCase.ExecutarAsync(clienteId, dto);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNullOrEmpty();

        _clienteRepositoryMock.Verify(x => x.AtualizarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Cliente>()), Times.Never);
    }
}
