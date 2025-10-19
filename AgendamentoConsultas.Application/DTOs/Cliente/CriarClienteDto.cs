namespace AgendamentoConsultas.Application.DTOs.Cliente;

public record CriarClienteDto(
    string Nome,
    string Cpf,
    string Email,
    string Telefone,
    DateTime DataNascimento
);
