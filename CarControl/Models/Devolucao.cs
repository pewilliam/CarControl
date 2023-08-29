using System;

namespace CarControl.Models
{
    class Devolucao
    {
        public int IdDevolucao { get; set; }
        public DateTime DhDevolucao{ get; set; }
        public int IdModelo { get; set; }
        public int IdCliente { get; set; }
        public int IdAluguel { get; set; }

        public Devolucao(int idDevolucao, DateTime dhDevolucao, int idModelo, int idCliente, int idAluguel)
        {
            IdDevolucao = idDevolucao;
            DhDevolucao = dhDevolucao;
            IdModelo = idModelo;
            IdCliente = idCliente;
            IdAluguel = idAluguel;
        }
    }
}
