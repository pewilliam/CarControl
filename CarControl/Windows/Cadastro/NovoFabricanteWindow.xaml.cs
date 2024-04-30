using MahApps.Metro.Controls;
using Npgsql;
using System.Windows;

namespace CarControl
{
    /// <summary>
    /// Lógica interna para NovoFabricanteWindow.xaml
    /// </summary>
    public partial class NovoFabricanteWindow : MetroWindow
    {
        static NpgsqlConnection conn = new NpgsqlConnection();

        public NovoFabricanteWindow(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            NomeFabricanteTxb.Focus();
        }

        private void SalvarCarroBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCampo())
            {
                string sql = ($"INSERT INTO fabricante(nome) VALUES ('{NomeFabricanteTxb.Text.ToUpper()}')");
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Fabricante inserido com sucesso!", "Fabricante inserido");
                Close();
            }
        }

        private bool ValidarCampo()
        {
            if (string.IsNullOrEmpty(NomeFabricanteTxb.Text))
            {
                MessageBox.Show("Preencha o nome do fabricante!", "Preenchimento");
                return false;
            }
            return true;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                Close();
            
            if (e.Key == System.Windows.Input.Key.Enter)
                SalvarCarroBtn_Click(sender, e);
            
        }

        private void FecharSaLVARCarroBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
