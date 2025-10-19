using AgendamentoConsultas.Application.DTOs.Cliente;
using AgendamentoConsultas.Domain.Entities;

namespace AgendamentoConsultas.Application.Extensions;

public static class ClienteExtensions
{
    public static ClienteDto ToDto(this Cliente cliente)
    {
        return new ClienteDto(
            cliente.Id,
            cliente.Nome,
            cliente.Cpf,
            cliente.Email,
            cliente.Telefone,
            cliente.DataNascimento,
            cliente.CriadoEm,
            cliente.AtualizadoEm
        );
    }
}
