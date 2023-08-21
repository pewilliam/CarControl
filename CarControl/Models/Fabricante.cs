namespace CarControl.Models
{
    internal class Fabricante
    {
        public int IdFabricante { get; set; }
        public string Nome { get; set; }
        public Fabricante(int idFabricante, string nome)
        {
            IdFabricante = idFabricante;
            Nome = nome;
        }
    }
}
