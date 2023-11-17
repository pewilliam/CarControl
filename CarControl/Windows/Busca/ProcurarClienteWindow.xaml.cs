using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para ProcurarClienteWindow.xaml
    /// </summary>
    public partial class ProcurarClienteWindow : MetroWindow
    {
        public int ClienteId { get; private set; }
        NpgsqlConnection conn = new NpgsqlConnection();
        List<Cliente> clientesList = new List<Cliente>();

        public ProcurarClienteWindow(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            MostrarClientes();
            searchClienteTxb.Focus();
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
                        reader.GetString(1).ToUpper(),
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
                MostrarClientes();
            else
            {
                var filteredList = clientesList.Where(x => x.Nome.ToLower().Contains(txb.Text.ToLower()));
                dg.ItemsSource = null;
                MostrarClientes();
                dg.ItemsSource = filteredList;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
            
            if (Keyboard.IsKeyDown(Key.N) && (Keyboard.IsKeyDown(Key.LeftAlt)))
                novoClienteBtn_Click(sender, e);
            
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                abrirClienteBtn_Click(sender, e); // Isso impede que o evento "Enter" seja processado
            
        }

        private void fecharBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }

        private void novoClienteBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NovoClienteWindow novoClienteWindow = new NovoClienteWindow(conn);
            novoClienteWindow.ShowDialog();
            novoClienteWindow.Owner = this;
            MostrarClientes();
        }

        private void abrirClienteBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Cliente c = dg.SelectedItem as Cliente;
            if (c is not null)
            {
                ClienteId = c.IdCliente;
                DialogResult = true;
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is Cliente selectedItem)
            {
                ClienteId = selectedItem.IdCliente;
                DialogResult = true;
            }
        }
    }
}
