using System;

namespace CarControl.Models
{
    class Devolucao
    {
        public int IdDevolucao { get; set; }
        
        public int IdModelo { get; set; }
        public string NomeModelo { get; set; }
        public int IdCliente { get; set; }
        public string NomeCliente { get; set; }
        public int IdAluguel { get; set; }
        public DateTime DhDevolucao { get; set; }

        public Devolucao(int idDevolucao, int idModelo, string nomeModelo, int idCliente, string nomeCliente, int idAluguel, DateTime dhDevolucao)
        {
            IdDevolucao = idDevolucao;
            IdModelo = idModelo;
            NomeModelo = nomeModelo;
            IdCliente = idCliente;
            NomeCliente = nomeCliente;
            IdAluguel = idAluguel;
            DhDevolucao = dhDevolucao;
        }
    }
}
