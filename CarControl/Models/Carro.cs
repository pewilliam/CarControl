﻿namespace CarControl.Models
{
    internal class Carro
    {
        public int IdCarro { get; set; }
        public string Nome { get; set; }

        public Carro(int idCarro, string nome, string v)
        {
            IdCarro = idCarro;
            Nome = nome;
        }
    }
}
