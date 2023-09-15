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
    /// Lógica interna para CategoriasWindows.xaml
    /// </summary>
    public partial class CategoriasWindows : MetroWindow
    {
        List<Categoria> categoriaList = new List<Categoria>();
        NpgsqlConnection conn = new NpgsqlConnection();

        public CategoriasWindows(NpgsqlConnection connection)
        {
            InitializeComponent();
            conn = connection;
            MostrarCategorias();
            SearchCategoriaTxb.Focus();
        }

        private void MostrarCategorias()
        {
            dg.ItemsSource = null;
            string sql = "SELECT * FROM carcontrol.categoria ORDER BY idcategoria;";

            categoriaList.Clear();

            NpgsqlCommand cmd = new(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Categoria categoria = new(
                        reader.GetInt32(0), //idfabricante
                        reader.GetString(1) //nome
                        );
                    categoriaList.Add(categoria);
                }
                reader.Close();
            }
            dg.ItemsSource = categoriaList;
            dg.Items.Refresh();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            if (Keyboard.IsKeyDown(Key.N) && (Keyboard.IsKeyDown(Key.LeftAlt)))
            {
                NovaCategoriaBtn_Click(sender, e);
            }
        }

        private void fecharBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NovaCategoriaBtn_Click(object sender, RoutedEventArgs e)
        {
            NovaCategoriaWindow novaCategoriaWindow = new NovaCategoriaWindow(conn);
            novaCategoriaWindow.ShowDialog();
            novaCategoriaWindow.Owner = this;
            MostrarCategorias();
        }

        private void SearchCategoriaTxb_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var txb = sender as TextBox;
            if (txb.Text == null)
            {
                MostrarCategorias();
            }
            else
            {
                var filteredList = categoriaList.Where(x => x.Nome.Contains(txb.Text));
                dg.ItemsSource = null;
                dg.ItemsSource = filteredList;
            }
        }
    }
}
