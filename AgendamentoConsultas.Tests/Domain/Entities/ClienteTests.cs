using AgendamentoConsultas.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AgendamentoConsultas.Tests.Domain.Entities;

public class ClienteTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarCliente()
    {
        var nome = "João Silva";
        var cpf = "123.456.789-09";
        var email = "joao@email.com";
        var telefone = "(11) 98765-4321";
        var dataNascimento = new DateTime(1990, 5, 15);

        var result = Cliente.Criar(nome, cpf, email, telefone, dataNascimento);

        result.IsSuccess.Should().BeTrue();
        result.Value.Nome.Should().Be(nome);
        result.Value.Cpf.Should().Be(cpf);
        result.Value.Email.Should().Be(email);
        result.Value.Telefone.Should().Be(telefone);
        result.Value.DataNascimento.Should().Be(dataNascimento);
    }

    [Theory]
    [InlineData("", "123.456.789-09", "joao@email.com", "(11) 98765-4321")]
    [InlineData(null, "123.456.789-09", "joao@email.com", "(11) 98765-4321")]
    [InlineData("   ", "123.456.789-09", "joao@email.com", "(11) 98765-4321")]
    public void Criar_ComNomeInvalido_DeveRetornarFalha(string nome, string cpf, string email, string telefone)
    {
        var dataNascimento = new DateTime(1990, 5, 15);

        var result = Cliente.Criar(nome, cpf, email, telefone, dataNascimento);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Nome não pode ser vazio");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("12345678901234")]
    [InlineData("111.111.111-11")]
    [InlineData("000.000.000-00")]
    public void Criar_ComCpfInvalido_DeveRetornarFalha(string cpf)
    {
        var nome = "João Silva";
        var email = "joao@email.com";
        var telefone = "(11) 98765-4321";
        var dataNascimento = new DateTime(1990, 5, 15);

        var result = Cliente.Criar(nome, cpf, email, telefone, dataNascimento);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Match(e => e.Contains("CPF") || e.Contains("vazio") || e.Contains("inválido"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Criar_ComEmailInvalido_DeveRetornarFalha(string email)
    {
        var nome = "João Silva";
        var cpf = "123.456.789-09";
        var telefone = "(11) 98765-4321";
        var dataNascimento = new DateTime(1990, 5, 15);

        var result = Cliente.Criar(nome, cpf, email, telefone, dataNascimento);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Email não pode ser vazio");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Criar_ComTelefoneInvalido_DeveRetornarFalha(string telefone)
    {
        var nome = "João Silva";
        var cpf = "123.456.789-09";
        var email = "joao@email.com";
        var dataNascimento = new DateTime(1990, 5, 15);

        var result = Cliente.Criar(nome, cpf, email, telefone, dataNascimento);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Telefone não pode ser vazio");
    }

    [Fact]
    public void Criar_ComDataNascimentoFutura_DeveRetornarFalha()
    {
        var nome = "João Silva";
        var cpf = "123.456.789-09";
        var email = "joao@email.com";
        var telefone = "(11) 98765-4321";
        var dataNascimento = DateTime.Today.AddDays(1);

        var result = Cliente.Criar(nome, cpf, email, telefone, dataNascimento);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Data de nascimento inválida");
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarCliente()
    {
        var cliente = Cliente.Criar("João Silva", "123.456.789-09", "joao@email.com", "(11) 98765-4321", new DateTime(1990, 5, 15)).Value;
        var novoNome = "João Silva Santos";
        var novoEmail = "joao.santos@email.com";
        var novoTelefone = "(11) 99999-8888";
        var novaDataNascimento = new DateTime(1990, 5, 15);

        var result = cliente.Atualizar(novoNome, novoEmail, novoTelefone, novaDataNascimento);

        result.IsSuccess.Should().BeTrue();
        cliente.Nome.Should().Be(novoNome);
        cliente.Email.Should().Be(novoEmail);
        cliente.Telefone.Should().Be(novoTelefone);
        cliente.DataNascimento.Should().Be(novaDataNascimento);
        cliente.AtualizadoEm.Should().NotBeNull();
    }

    [Theory]
    [InlineData("", "joao@email.com", "(11) 98765-4321")]
    [InlineData(null, "joao@email.com", "(11) 98765-4321")]
    public void Atualizar_ComNomeInvalido_DeveRetornarFalha(string nome, string email, string telefone)
    {
        var cliente = Cliente.Criar("João Silva", "123.456.789-09", "joao@email.com", "(11) 98765-4321", new DateTime(1990, 5, 15)).Value;
        var dataNascimento = new DateTime(1990, 5, 15);

        var result = cliente.Atualizar(nome, email, telefone, dataNascimento);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Nome não pode ser vazio");
    }

    [Fact]
    public void Atualizar_ComDataNascimentoFutura_DeveRetornarFalha()
    {
        var cliente = Cliente.Criar("João Silva", "123.456.789-09", "joao@email.com", "(11) 98765-4321", new DateTime(1990, 5, 15)).Value;
        var dataNascimento = DateTime.Today.AddDays(1);

        var result = cliente.Atualizar("João Silva", "joao@email.com", "(11) 98765-4321", dataNascimento);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Data de nascimento inválida");
    }
}
