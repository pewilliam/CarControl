using System;

namespace CarControl.Models
{
    internal class Recebimento
    {
        public int IdRecebimento { get; set; }
        public string NomeCliente { get; set; }
        public double ValorOriginal { get; set; }
        public double? ValorRecebido { get; set; }
        public DateOnly RecebimentoDiaPrevisto { get; set; }
        public DateTime? DhRecebimento { get; set; }
        public bool EmAberto { get; set; }

        public Recebimento(int idRecebimento, string nomeCliente, double valorOriginal, double? valorRecebido, DateOnly recebimentoDiaPrevisto, DateTime? dhRecebimento, bool emAberto)
        {
            IdRecebimento = idRecebimento;
            NomeCliente = nomeCliente;
            ValorOriginal = valorOriginal;
            ValorRecebido = valorRecebido;
            RecebimentoDiaPrevisto = recebimentoDiaPrevisto;
            DhRecebimento = dhRecebimento;
            EmAberto = emAberto;
        }
    }
}
