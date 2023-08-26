using MahApps.Metro.Controls;
using Npgsql;
using System.Windows;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para NovoUsuarioWindow.xaml
    /// </summary>
    public partial class NovoUsuarioWindow : MetroWindow
    {
        NpgsqlConnection conn = new();

        public NovoUsuarioWindow(NpgsqlConnection connection)
        {
            InitializeComponent();
            txbUser.Focus();
            conn = connection;
        }

        private void btnSalvar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string login = txbUser.Text;
            string password = txbPassword.Password;

            string sql = $"CREATE USER {login} SUPERUSER PASSWORD '{password}';";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Usuário inserido com sucesso!");
            Close();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
