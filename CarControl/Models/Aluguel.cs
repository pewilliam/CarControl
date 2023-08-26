using System;

namespace CarControl.Models
{
    class Aluguel
    {
        public int IdAluguel { get; set; }
        public int IdCliente { get; set; }
        public int IdModelo { get; set; }
        public int IdFormaPagto { get; set; }
        public DateTime DhAluguel { get; set; }
        public int DiasAluguel { get; set; }
        public double ValorAluguel { get; set; }

        public Aluguel(int idAluguel, int idCliente, int idModelo, int idFormaPagto, DateTime dhAluguel, int diasAluguel, double valorAluguel)
        {
            IdAluguel = idAluguel;
            IdCliente = idCliente;
            IdModelo = idModelo;
            IdFormaPagto = idFormaPagto;
            DhAluguel = dhAluguel;
            DiasAluguel = diasAluguel;
            ValorAluguel = valorAluguel;
        }
    }
}
