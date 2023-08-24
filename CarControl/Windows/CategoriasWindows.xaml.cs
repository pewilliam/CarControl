using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System.Collections.Generic;
using System.Windows;

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
    }
}
