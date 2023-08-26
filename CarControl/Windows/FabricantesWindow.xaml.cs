using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System.Collections.Generic;
using System.Windows;
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

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
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
    }
}
