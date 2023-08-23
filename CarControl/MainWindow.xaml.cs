using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        static NpgsqlConnection conn = new NpgsqlConnection();
        static List<Carro> carroList = new List<Carro>();

        public MainWindow()
        {
            InitializeComponent();
            searchCarTxb.Focus();
            MostrarCarros();
        }

        private void MostrarCarros()
        {
            dg.ItemsSource = null;
            //string connection = "Server=localhost;Port=5432;Database=base_carros;User id=postgres;Password=pedrow2001";
            string connection = "Server=localhost;Port=5433;Database=base_carros;User id=postgres;Password=pedrow2001";
            conn.ConnectionString = connection;
            string sql = "SELECT * FROM carcontrol.carro;";

            conn.Open();

            carroList.Clear();

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
                conn.Close();
            }
            dg.ItemsSource = carroList;
            dg.Items.Refresh();
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

        private void novoCarroBtn_Click(object sender, RoutedEventArgs e)
        {
            NovoCarroWindow novoCarroWindow = new NovoCarroWindow();
            novoCarroWindow.ShowDialog();
            MostrarCarros();
        }

        private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            Carro c = row.DataContext as Carro;
            ModelosWindow modelosWindow = new ModelosWindow(c.IdCarro);
            modelosWindow.ShowDialog();
        }

        private void abrirCarroBtn_Click(object sender, RoutedEventArgs e)
        {
            Carro c = dg.SelectedItem as Carro;
            if (c is not null)
            {
                ModelosWindow modelosWindow = new ModelosWindow(c.IdCarro);
                modelosWindow.ShowDialog();
            }
        }
    }
}
