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
    /// Lógica interna para FabricantesWindow.xaml
    /// </summary>
    public partial class FabricantesWindow : MetroWindow
    {
        List<Fabricante> fabricantesList = new List<Fabricante>();
        NpgsqlConnection conn = new NpgsqlConnection();

        public FabricantesWindow(NpgsqlConnection connection)
        {
            InitializeComponent();
            conn = connection;
            MostrarFabricantes();
            SearchFabricanteTxb.Focus();
        }

        private void MostrarFabricantes()
        {
            dg.ItemsSource = null;
            string sql = "SELECT * FROM carcontrol.fabricante ORDER BY idfabricante;";

            fabricantesList.Clear();

            NpgsqlCommand cmd = new(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Fabricante fabricante = new(
                        reader.GetInt32(0), //idfabricante
                        reader.GetString(1) //nome
                        );
                    fabricantesList.Add(fabricante);
                }
                reader.Close();
            }
            dg.ItemsSource = fabricantesList;
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
                NovoFabricanteBtn_Click(sender, e);
            }
        }

        private void fecharBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NovoFabricanteBtn_Click(object sender, RoutedEventArgs e)
        {
            NovoFabricanteWindow novoFabricanteWindow = new NovoFabricanteWindow(conn);
            novoFabricanteWindow.ShowDialog();
            novoFabricanteWindow.Owner = this;
            MostrarFabricantes();
        }

        private void SearchFabricanteTxb_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var txb = sender as TextBox;
            if (txb.Text == null)
            {
                MostrarFabricantes();
            }
            else
            {
                var filteredList = fabricantesList.Where(x => x.Nome.Contains(txb.Text));
                dg.ItemsSource = null;
                dg.ItemsSource = filteredList;
            }
        }
    }
}
