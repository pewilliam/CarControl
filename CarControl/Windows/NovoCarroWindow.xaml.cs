using Npgsql;
using MahApps.Metro.Controls;
using System.Windows;

namespace CarControl
{
    /// <summary>
    /// Lógica interna para NovoCarroWindow.xaml
    /// </summary>
    public partial class NovoCarroWindow : MetroWindow
    {
        static NpgsqlConnection conn = new NpgsqlConnection();

        public NovoCarroWindow(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            NomeCarroTxb.Focus();
        }

        private void FecharSaLVARCarroBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SalvarCarroBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCampo())
            {
                string sql = ($"INSERT INTO carcontrol.carro(nome) VALUES('{NomeCarroTxb.Text.ToUpper()}');");

                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Carro inserido com sucesso!");
                Close();
            }
        }

        private bool ValidarCampo()
        {
            if (string.IsNullOrEmpty(NomeCarroTxb.Text))
            {
                MessageBox.Show("Preencha o nome do carro!", "Preenchimento");
                return false;
            }
            return true;
        }
    }
}
