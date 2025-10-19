namespace AgendamentoConsultas.Application.DTOs.Cliente;

public record ClienteDto(
    Guid Id,
    string Nome,
    string Cpf,
    string Email,
    string Telefone,
    DateTime DataNascimento,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
