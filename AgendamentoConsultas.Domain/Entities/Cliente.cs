using AgendamentoConsultas.Domain.Common;
using AgendamentoConsultas.Domain.Patterns;

namespace AgendamentoConsultas.Domain.Entities;

public class Cliente : BaseEntity
{
    public string Nome { get; private set; }
    public string Cpf { get; private set; }
    public string Email { get; private set; }
    public string Telefone { get; private set; }
    public DateTime DataNascimento { get; private set; }

    private Cliente() { }

    private Cliente(string nome, string cpf, string email, string telefone, DateTime dataNascimento)
    {
        Nome = nome;
        Cpf = cpf;
        Email = email;
        Telefone = telefone;
        DataNascimento = dataNascimento;
    }

    public static Result<Cliente> Criar(string nome, string cpf, string email, string telefone, DateTime dataNascimento)
    {
        var notification = new Notification();
        
        ValidarDados(nome, cpf, email, telefone, dataNascimento, notification);
        
        if (notification.HasErrors)
            return Result.Failure<Cliente>(notification.GetErrorsAsString());
        
        var cliente = new Cliente(nome, cpf, email, telefone, dataNascimento);
        return Result.Success(cliente);
    }

    public Result Atualizar(string nome, string email, string telefone, DateTime dataNascimento)
    {
        var notification = new Notification();
        
        if (string.IsNullOrWhiteSpace(nome))
            notification.AddError("Nome não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(email))
            notification.AddError("Email não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(telefone))
            notification.AddError("Telefone não pode ser vazio");
        
        if (dataNascimento >= DateTime.Today)
            notification.AddError("Data de nascimento inválida");

        if (notification.HasErrors)
            return Result.Failure(notification.GetErrorsAsString());

        Nome = nome;
        Email = email;
        Telefone = telefone;
        DataNascimento = dataNascimento;
        AtualizarDataModificacao();
        
        return Result.Success();
    }

    private static void ValidarDados(string nome, string cpf, string email, string telefone, DateTime dataNascimento, Notification notification)
    {
        if (string.IsNullOrWhiteSpace(nome))
            notification.AddError("Nome não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(cpf))
            notification.AddError("CPF não pode ser vazio");
        else if (!ValidarCpf(cpf))
            notification.AddError("CPF inválido");
        
        if (string.IsNullOrWhiteSpace(email))
            notification.AddError("Email não pode ser vazio");
        
        if (string.IsNullOrWhiteSpace(telefone))
            notification.AddError("Telefone não pode ser vazio");
        
        if (dataNascimento >= DateTime.Today)
            notification.AddError("Data de nascimento inválida");
    }

    private static bool ValidarCpf(string cpf)
    {
        cpf = cpf.Replace(".", "").Replace("-", "").Trim();
        
        if (cpf.Length != 11)
            return false;
        
        if (!cpf.All(char.IsDigit))
            return false;
        
        if (cpf.Distinct().Count() == 1)
            return false;

        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        
        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        string digito = resto.ToString();
        tempCpf += digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        digito += resto.ToString();

        return cpf.EndsWith(digito);
    }
}
