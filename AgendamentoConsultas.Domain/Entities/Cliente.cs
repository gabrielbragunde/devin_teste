using AgendamentoConsultas.Domain.Common;

namespace AgendamentoConsultas.Domain.Entities;

public class Cliente : BaseEntity
{
    public string Nome { get; private set; }
    public string Cpf { get; private set; }
    public string Email { get; private set; }
    public string Telefone { get; private set; }
    public DateTime DataNascimento { get; private set; }

    private Cliente() { }

    public Cliente(string nome, string cpf, string email, string telefone, DateTime dataNascimento)
    {
        ValidarDados(nome, cpf, email, telefone, dataNascimento);
        
        Nome = nome;
        Cpf = cpf;
        Email = email;
        Telefone = telefone;
        DataNascimento = dataNascimento;
    }

    public void Atualizar(string nome, string email, string telefone, DateTime dataNascimento)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome não pode ser vazio", nameof(nome));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email não pode ser vazio", nameof(email));
        
        if (string.IsNullOrWhiteSpace(telefone))
            throw new ArgumentException("Telefone não pode ser vazio", nameof(telefone));
        
        if (dataNascimento >= DateTime.Today)
            throw new ArgumentException("Data de nascimento inválida", nameof(dataNascimento));

        Nome = nome;
        Email = email;
        Telefone = telefone;
        DataNascimento = dataNascimento;
        AtualizarDataModificacao();
    }

    private void ValidarDados(string nome, string cpf, string email, string telefone, DateTime dataNascimento)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome não pode ser vazio", nameof(nome));
        
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF não pode ser vazio", nameof(cpf));
        
        if (!ValidarCpf(cpf))
            throw new ArgumentException("CPF inválido", nameof(cpf));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email não pode ser vazio", nameof(email));
        
        if (string.IsNullOrWhiteSpace(telefone))
            throw new ArgumentException("Telefone não pode ser vazio", nameof(telefone));
        
        if (dataNascimento >= DateTime.Today)
            throw new ArgumentException("Data de nascimento inválida", nameof(dataNascimento));
    }

    private bool ValidarCpf(string cpf)
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
