using CarControl.Models;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using System.Windows.Input;
using System;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para ClientesWindow.xaml
    /// </summary>
    public partial class ClientesWindow : MetroWindow
    {
        NpgsqlConnection conn = new NpgsqlConnection();
        List<Cliente> clientesList = new List<Cliente>();

        public ClientesWindow(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            MostrarClientes();
        }

        private void MostrarClientes()
        {
            dg.ItemsSource = null;
            string sql = "SELECT * FROM carcontrol.cliente ORDER BY idcliente;";

            clientesList.Clear();

            NpgsqlCommand cmd = new(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Cliente cliente = new(
                        reader.GetInt32(0), //idcarro
                        reader.GetString(1),
                        Convert.ToUInt64(reader.GetString(2)).ToString(@"000\.000\.000\-00"),
                        reader.GetString(3),
                        reader.GetDateTime(4)
                        );
                    clientesList.Add(cliente);
                }
                reader.Close();
            }
            dg.ItemsSource = clientesList;
            dg.Items.Refresh();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txb = sender as TextBox;
            if (txb.Text == null)
            {
                MostrarClientes();
            }
            else
            {
                var filteredList = clientesList.Where(x => x.Nome.ToLower().Contains(txb.Text.ToLower()));
                dg.ItemsSource = null;
                MostrarClientes();
                dg.ItemsSource = filteredList;
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Isso impede que o evento "Enter" seja processado
            }
        }

        private void fecharBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
    }
}
