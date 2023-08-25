namespace CarControl.Models
{
    class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; }

        public Usuario(int idUsuario, string nome)
        {
            IdUsuario = idUsuario;
            Nome = nome;
        }
    }
}
