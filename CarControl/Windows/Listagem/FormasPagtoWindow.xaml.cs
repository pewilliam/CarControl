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
    /// Lógica interna para FormasPagtoWindow.xaml
    /// </summary>
    public partial class FormasPagtoWindow : MetroWindow
    {
        NpgsqlConnection conn = new NpgsqlConnection();
        List<FormaPagto> formaPagtoList = new List<FormaPagto>();

        public FormasPagtoWindow(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            MostrarFormasPagto();
            SearchFormaPagtoTxb.Focus();
        }

        private void MostrarFormasPagto()
        {
            dg.ItemsSource = null;
            string sql = "SELECT * FROM formapagto ORDER BY idformapagto;";

            formaPagtoList.Clear();

            NpgsqlCommand cmd = new(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    FormaPagto formaPagto = new(
                        reader.GetInt32(0),
                        reader.GetString(1).ToUpper()
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();

            if (Keyboard.IsKeyDown(Key.N) && (Keyboard.IsKeyDown(Key.LeftAlt)))
                novaFormaPagtoBtn_Click(sender, e);
        }

        private void fecharBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SearchFormaPagtoTxb_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var txb = sender as TextBox;
            if (txb.Text == null)
                MostrarFormasPagto();

            else
            {
                var filteredList = formaPagtoList.Where(x => x.Nome.Contains(txb.Text));
                dg.ItemsSource = null;
                dg.ItemsSource = filteredList;
            }
        }
    }
}
