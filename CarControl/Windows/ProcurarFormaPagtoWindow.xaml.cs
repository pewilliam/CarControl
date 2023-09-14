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
    /// Lógica interna para ProcurarFormaPagto.xaml
    /// </summary>
    public partial class ProcurarFormaPagto : MetroWindow
    {
        public int FormaPagtoId { get; private set; }
        NpgsqlConnection conn = new NpgsqlConnection();
        List<FormaPagto> formaPagtoList = new List<FormaPagto>();

        public ProcurarFormaPagto(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            MostrarFormasPagto();
            SearchFormaPagtoTxb.Focus();
        }

        private void MostrarFormasPagto()
        {
            dg.ItemsSource = null;
            string sql = "SELECT * FROM carcontrol.formapagto ORDER BY idformapagto;";

            formaPagtoList.Clear();

            NpgsqlCommand cmd = new(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    FormaPagto formaPagto = new(
                        reader.GetInt32(0), //idcarro
                        reader.GetString(1).ToUpper() //nome
                        );
                    formaPagtoList.Add(formaPagto);
                }
                reader.Close();
            }
            dg.ItemsSource = formaPagtoList;
            dg.Items.Refresh();
        }

        private void novaFormaPagtoBtn_Click(object sender, RoutedEventArgs e)
        {
            NovaFormaPagtoWindow novaFormaPagtoWindow = new NovaFormaPagtoWindow(conn);
            novaFormaPagtoWindow.ShowDialog();
            novaFormaPagtoWindow.Owner = this;
            MostrarFormasPagto();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
            if (Keyboard.IsKeyDown(Key.N) && (Keyboard.IsKeyDown(Key.LeftAlt)))
            {
                novaFormaPagtoBtn_Click(sender, e);
            }
        }

        private void fecharBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is FormaPagto selectedItem)
            {
                FormaPagtoId = selectedItem.IdFormaPagto;
                DialogResult = true;
            }
        }

        private void abrirFormaPagtoBtn_Click(object sender, RoutedEventArgs e)
        {
            FormaPagto fp = dg.SelectedItem as FormaPagto;
            if (fp is not null)
            {
                FormaPagtoId = fp.IdFormaPagto;
                DialogResult = true;
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                abrirFormaPagtoBtn_Click(sender, e); // Isso impede que o evento "Enter" seja processado
            }
        }

        private void SearchFormaPagtoTxb_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var txb = sender as TextBox;
            if (txb.Text == null)
            {
                MostrarFormasPagto();
            }
            else
            {
                var filteredList = formaPagtoList.Where(x => x.Nome.Contains(txb.Text));
                dg.ItemsSource = null;
                dg.ItemsSource = filteredList;
            }
        }
    }
}
