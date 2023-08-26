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
                MessageBox.Show("Forma de pagamento inserida com sucesso!");
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

        private void FecharSalvarFabricanteBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
