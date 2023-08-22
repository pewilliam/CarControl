using CarControl.Models;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            carroList.Clear();
            string connection = "Server=localhost;Port=5432;Database=base_carros;User id=postgres;Password=pedrow2001";
            conn.ConnectionString = connection;
            string sql = "SELECT * FROM carcontrol.carro;";

            conn.Open();

            NpgsqlCommand cmd = new(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
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
            conn.Close();
        }

        private void DataGridRow_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is Carro selectedItem)
            {
                ModelosWindow modeloWindow = new ModelosWindow(selectedItem);
                modeloWindow.ShowDialog();
                modeloWindow.Owner = this;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txb = sender as TextBox;
            if (txb.Text == null)
            {
                MostrarCarros();
            }
            else
            {
                var filteredList = carroList.Where(x => x.Nome.ToLower().Contains(txb.Text.ToLower()));
                dg.ItemsSource = null;
                MostrarCarros();
                dg.ItemsSource = filteredList;
            }
        }

        private void fecharBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
