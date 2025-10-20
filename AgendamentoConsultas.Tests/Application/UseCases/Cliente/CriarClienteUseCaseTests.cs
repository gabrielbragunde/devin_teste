using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Application.Interfaces;
using AgendamentoConsultas.Application.UseCases.Cliente;
using AgendamentoConsultas.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace AgendamentoConsultas.Tests.Application.UseCases.Cliente;

public class CriarClienteUseCaseTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly CriarClienteUseCase _useCase;

    public CriarClienteUseCaseTests()
    {
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _useCase = new CriarClienteUseCase(_clienteRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecutarAsync_ComDadosValidos_DeveCriarCliente()
    {
        var dto = new CriarClienteDto(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        );

        _clienteRepositoryMock
            .Setup(x => x.ObterPorCpfAsync(It.IsAny<string>()))
            .ReturnsAsync((AgendamentoConsultas.Domain.Entities.Cliente?)null);

        _clienteRepositoryMock
            .Setup(x => x.AdicionarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Cliente>()))
            .ReturnsAsync((AgendamentoConsultas.Domain.Entities.Cliente c) => c);

        var result = await _useCase.ExecutarAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value.Nome.Should().Be(dto.Nome);
        result.Value.Cpf.Should().Be(dto.Cpf);
        result.Value.Email.Should().Be(dto.Email);
        result.Value.Telefone.Should().Be(dto.Telefone);
        result.Value.DataNascimento.Should().Be(dto.DataNascimento);

        _clienteRepositoryMock.Verify(x => x.ObterPorCpfAsync(dto.Cpf), Times.Once);
        _clienteRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Cliente>()), Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_ComCpfJaExistente_DeveRetornarFalha()
    {
        var dto = new CriarClienteDto(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            new DateTime(1990, 5, 15)
        );

        var clienteExistente = AgendamentoConsultas.Domain.Entities.Cliente.Criar(
            "Maria Silva",
            "123.456.789-09",
            "maria@email.com",
            "(11) 98765-4321",
            new DateTime(1985, 3, 10)
        ).Value;

        _clienteRepositoryMock
            .Setup(x => x.ObterPorCpfAsync(It.IsAny<string>()))
            .ReturnsAsync(clienteExistente);

        var result = await _useCase.ExecutarAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Já existe um cliente cadastrado com este CPF");

        _clienteRepositoryMock.Verify(x => x.ObterPorCpfAsync(dto.Cpf), Times.Once);
        _clienteRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Cliente>()), Times.Never);
    }

    [Theory]
    [InlineData("", "123.456.789-09", "joao@email.com", "(11) 98765-4321")]
    [InlineData("João", "123", "joao@email.com", "(11) 98765-4321")]
    [InlineData("João", "123.456.789-09", "", "(11) 98765-4321")]
    [InlineData("João", "123.456.789-09", "joao@email.com", "")]
    public async Task ExecutarAsync_ComDadosInvalidos_DeveRetornarFalha(string nome, string cpf, string email, string telefone)
    {
        var dto = new CriarClienteDto(
            nome,
            cpf,
            email,
            telefone,
            new DateTime(1990, 5, 15)
        );

        var result = await _useCase.ExecutarAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNullOrEmpty();

        _clienteRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Cliente>()), Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_ComDataNascimentoFutura_DeveRetornarFalha()
    {
        var dto = new CriarClienteDto(
            "João Silva",
            "123.456.789-09",
            "joao@email.com",
            "(11) 98765-4321",
            DateTime.Today.AddDays(1)
        );

        var result = await _useCase.ExecutarAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Data de nascimento inválida");

        _clienteRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<AgendamentoConsultas.Domain.Entities.Cliente>()), Times.Never);
    }
}
