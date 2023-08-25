namespace CarControl.Models
{
    internal class FormaPagto
    {
        public int IdFormaPagto { get; set; }
        public string Nome { get; set; }

        public FormaPagto(int idFormaPagto, string nome)
        {
            IdFormaPagto = idFormaPagto;
            Nome = nome;
        }
    }
}
