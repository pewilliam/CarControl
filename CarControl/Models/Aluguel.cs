using System;

namespace CarControl.Models
{
    class Aluguel
    {
        public int IdAluguel { get; set; }
        public int IdCliente { get; set; }
        public string NomeCliente { get; set; }
        public int IdModelo { get; set; }
        public string NomeModelo { get; set; }
        public int IdFormaPagto { get; set; }
        public string NomeFormaPagto { get; set; }
        public DateTime DhAluguel { get; set; }
        public int DiasAluguel { get; set; }
        public double ValorAluguel { get; set; }
        public bool EmAndamento { get; set; }

        public Aluguel(int idAluguel, int idCliente, string nomeCliente, int idModelo, string nomeModelo, int idFormaPagto, string nomeFormaPagto, DateTime dhAluguel, int diasAluguel, double valorAluguel, bool emAndamento)
        {
            IdAluguel = idAluguel;
            IdCliente = idCliente;
            NomeCliente = nomeCliente;
            IdModelo = idModelo;
            NomeModelo = nomeModelo;
            IdFormaPagto = idFormaPagto;
            NomeFormaPagto = nomeFormaPagto;
            DhAluguel = dhAluguel;
            DiasAluguel = diasAluguel;
            ValorAluguel = valorAluguel;
            EmAndamento = emAndamento;
        }
    }
}
