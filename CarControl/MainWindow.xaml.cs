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
        static List<Carro> carroList = new List<Carro>();

        public MainWindow()
        {
            InitializeComponent();
            MostrarCarros();
        }

        private void MostrarCarros()
        {
            string connection = "Server=localhost;Port=5432;Database=base_carros;User id=postgres;Password=pedrow2001";
            conn.ConnectionString = connection;
            string sql = "SELECT * FROM carcontrol.carro;";

            conn.Open();

            NpgsqlCommand cmd = new(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while(reader.Read())
                {
                    Carro carro = new(
                        reader.GetInt32(0), //idcarro
                        reader.GetString(1) //nome
                        );
                    carroList.Add(carro);
                }
                reader.Close();
                dg.ItemsSource = carroList;
            }
        }


    }
}
