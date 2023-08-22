namespace CarControl.Models
{
    public class Modelo
    {
        public int IdModelo { get; set; }
        public string Nome { get; set; }
        public string Cor { get; set; }
        public int QtdPortas { get; set; }
        public int QtdPassageiros { get; set; }
        public string Combustivel { get; set; }
        public string Placa { get; set; }
        public string Ano { get; set; }
        public string TipoCambio { get; set; }
        public double Preco { get; set; }
        public int IdCarro { get; set; }
        public int IdFabricante { get; set; }
        public int IdCategoria { get; set; }

        public Modelo(int idModelo, string nome, string cor, int qtdPortas, int qtdPassageiros, string combustivel, string placa, string ano, string tipoCambio, double preco, int idCarro, int idFabricante, int idCategoria)
        {
            IdModelo = idModelo;
            Nome = nome;
            Cor = cor;
            QtdPortas = qtdPortas;
            QtdPassageiros = qtdPassageiros;
            Combustivel = combustivel;
            Placa = placa;
            Ano = ano;
            TipoCambio = tipoCambio;
            Preco = preco;
            IdCarro = idCarro;
            IdFabricante = idFabricante;
            IdCategoria = idCategoria;
        }
    }
}
