using MahApps.Metro.Controls;
using Npgsql;
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

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SalvarCarroBtn_Click(sender, e);
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
