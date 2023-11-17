using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para UsuariosWindow.xaml
    /// </summary>
    public partial class UsuariosWindow : MetroWindow
    {
        List<Usuario> userList = new List<Usuario>();
        NpgsqlConnection conn;

        public UsuariosWindow(NpgsqlConnection connection)
        {
            InitializeComponent();
            conn = connection;
            MostrarUsuarios();
        }

        private void MostrarUsuarios()
        {
            dg.ItemsSource = null;
            string sql = "SELECT ROW_NUMBER() OVER () AS iduser, UPPER(rolname) AS nome FROM pg_roles WHERE rolcanlogin";

            userList.Clear();

            NpgsqlCommand cmd = new(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Usuario usuario = new(
                        reader.GetInt32(0),
                        reader.GetString(1).ToUpper()
                        );
                    userList.Add(usuario);
                }
                reader.Close();
            }
            dg.ItemsSource = userList;
            dg.Items.Refresh();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();

            if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.N))
                novoUsuarioBtn_Click(sender, e);

        }

        private void fecharBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void novoUsuarioBtn_Click(object sender, RoutedEventArgs e)
        {
            NovoUsuarioWindow novoUsuarioWindow = new NovoUsuarioWindow(conn);
            novoUsuarioWindow.ShowDialog();
            novoUsuarioWindow.Owner = this;
            MostrarUsuarios();
        }
    }
}
