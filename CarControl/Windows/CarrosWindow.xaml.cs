using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para CarrosWindow.xaml
    /// </summary>
    public partial class CarrosWindow : MetroWindow
    {
        List<Carro> carroList = new List<Carro>();
        NpgsqlConnection conn;

        public CarrosWindow(NpgsqlConnection connection)
        {
            InitializeComponent();
            conn = connection;
            MostrarCarros();
            searchCarTxb.Focus();
        }

        private void MostrarCarros()
        {
            dg.ItemsSource = null;
            string sql = "SELECT * FROM carcontrol.carro ORDER BY idcarro;";

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

        private void novoCarroBtn_Click(object sender, RoutedEventArgs e)
        {
            NovoCarroWindow novoCarroWindow = new NovoCarroWindow(conn);
            novoCarroWindow.ShowDialog();
            MostrarCarros();
        }

        private void DataGridRow_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is Carro selectedItem)
            {
                ModelosWindow modelosWindow = new ModelosWindow(conn);
                modelosWindow.CarrosCB.SelectedValue = selectedItem.IdCarro;
                modelosWindow.ShowDialog();
            }
        }

        private void abrirCarroBtn_Click(object sender, RoutedEventArgs e)
        {
            Carro c = dg.SelectedItem as Carro;
            if (c is not null)
            {
                ModelosWindow modelosWindow = new ModelosWindow(conn);
                modelosWindow.CarrosCB.SelectedValue = c.IdCarro;
                modelosWindow.ShowDialog();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            if (Keyboard.IsKeyDown(Key.N) && (Keyboard.IsKeyDown(Key.LeftAlt)))
            {
                novoCarroBtn_Click(sender, e);
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                abrirCarroBtn_Click(sender, e); // Isso impede que o evento "Enter" seja processado
            }
        }

        private void fecharBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
