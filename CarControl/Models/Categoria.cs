namespace CarControl.Models
{
    class Categoria
    {
        public int IdCategoria { get; set; }
        public string Nome { get; set; }

        public Categoria(int idCategoria, string nome)
        {
            IdCategoria = idCategoria;
            Nome = nome;
        }
    }
}
