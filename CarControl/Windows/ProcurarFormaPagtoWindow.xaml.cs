using CarControl.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para ProcurarFormaPagto.xaml
    /// </summary>
    public partial class ProcurarFormaPagto : Window
    {
        public int FormaPagtoId { get; private set; }
        NpgsqlConnection conn = new NpgsqlConnection();
        List<FormaPagto> formaPagtoList = new List<FormaPagto>();

        public ProcurarFormaPagto(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            MostrarFormasPagto();
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
    }
}
