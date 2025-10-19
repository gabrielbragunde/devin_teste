namespace AgendamentoConsultas.Application.DTOs.Cliente;

public record AtualizarClienteDto(
    string Nome,
    string Email,
    string Telefone,
    DateTime DataNascimento
);
