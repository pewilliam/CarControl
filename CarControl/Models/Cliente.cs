using System;

namespace CarControl.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public DateTime DtNascimento { get; set; }

        public Cliente(int idCliente, string nome, string cpf, string email, DateTime dtNascimento)
        {
            IdCliente = idCliente;
            Nome = nome;
            Cpf = cpf;
            Email = email;
            DtNascimento = dtNascimento;
        }
    }
}
