namespace CarControl.Models
{
    public class Carro
    {
        public int IdCarro { get; set; }
        public string Nome { get; set; }

        public Carro(int idCarro, string nome)
        {
            IdCarro = idCarro;
            Nome = nome;
        }
    }
}
