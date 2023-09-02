using MahApps.Metro.Controls;
using Npgsql;
using System.Windows;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para NovaFormaPagtoWindow.xaml
    /// </summary>
    public partial class NovaFormaPagtoWindow : MetroWindow
    {
        NpgsqlConnection conn = new NpgsqlConnection();

        public NovaFormaPagtoWindow(NpgsqlConnection connection)
        {
            InitializeComponent();
            NomeFormaPagtoTxb.Focus();
            conn = connection;
        }

        private void SalvarFormaPatgoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCampo())
            {
                string sql = ($"INSERT INTO carcontrol.formapagto(nome) VALUES ('{NomeFormaPagtoTxb.Text.ToUpper()}')");
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Forma de pagamento inserida com sucesso!", "Forma de pagamento inserida");
                Close();
            }
        }

        private bool ValidarCampo()
        {
            if (string.IsNullOrEmpty(NomeFormaPagtoTxb.Text))
            {
                MessageBox.Show("Preencha o nome da forma de pagamento!", "Preenchimento");
                return false;
            }
            return true;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SalvarFormaPatgoBtn_Click(sender, e);
            }
        }

        private void FecharSalvarFabricanteBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
