using MahApps.Metro.Controls;
using Npgsql;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para NovoClienteWindow.xaml
    /// </summary>
    public partial class NovoClienteWindow : MetroWindow
    {
        NpgsqlConnection conn = new NpgsqlConnection();

        public NovoClienteWindow(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
        }

        private void CpfTxb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (CpfTxb.Text != string.Empty)
            {
                CpfTxb.Text = new string(CpfTxb.Text.Where(char.IsDigit).ToArray());
                CpfTxb.CaretIndex = CpfTxb.Text.Length;
            }
        }

        private void CpfTxb_LostFocus(object sender, RoutedEventArgs e)
        {
            if(CpfTxb.Text != string.Empty)
            {
                long CPF = Convert.ToInt64(CpfTxb.Text);
                string CPFFormatado = String.Format(@"{0:\000\.000\.000\-00}", CPF);
                CpfTxb.Text = CPFFormatado;
            }
        }

        private void SalvarNovoModeloBtn_Click(object sender, RoutedEventArgs e)
        {
            string sql = $"INSERT INTO carcontrol.cliente(nome, cpf, email, dtnascimento) VALUES (UPPER('{NomeClienteTxb.Text}'), '{CpfTxb.Text = new string(CpfTxb.Text.Where(char.IsDigit).ToArray())}', '{EmailTxb.Text}', '{DataNascimentoTxb.Text}')";
            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Cliente inserido com sucesso!", "Sucesso!");
            Close();
        }

        private void FecharNovoModeloWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }
    }
}
