using Npgsql;
using System.Windows;
using CarControl.Models;
using System.Collections.Generic;

namespace CarControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static NpgsqlConnection conn = new NpgsqlConnection();
        static List<Modelo> modeloList = new List<Modelo>();

        public MainWindow()
        {
            InitializeComponent();
            MostrarCarros();
        }

        private void MostrarCarros()
        {
            string connection = "Server=localhost;Port=5432;Database=base_carros;User id=postgres;Password=pedrow2001";
            conn.ConnectionString = connection;
            string sql = "SELECT * FROM carcontrol.modelo;";

            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while(reader.Read())
                {
                    Modelo modelo = new(
                        reader.GetInt32(0), //id
                        reader.GetString(1), //nome
                        reader.GetString(2), //cor
                        reader.GetInt32(3), //qtdportas
                        reader.GetInt32(4), //qtdpassageiros
                        reader.GetString(5), //combustivel
                        reader.GetString(6), //placa
                        reader.GetString(7), //ano
                        reader.GetString(8), //tipocambio
                        reader.GetDouble(9), //preco
                        reader.GetInt32(10), //idcarro
                        reader.GetInt32(11), //idfabricante
                        reader.GetInt32(12) //idfabricante
                        );
                    modeloList.Add(modelo);
                }
                reader.Close();
                dg.ItemsSource = modeloList;
            }
        }
    }
}
