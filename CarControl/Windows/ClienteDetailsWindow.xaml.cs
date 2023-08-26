using CarControl.Models;
using Npgsql;
using MahApps.Metro.Controls;
using System.Linq;
using System.Windows;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para ClienteDetailsWindow.xaml
    /// </summary>
    public partial class ClienteDetailsWindow : MetroWindow
    {
        NpgsqlConnection conn = new NpgsqlConnection();

        public ClienteDetailsWindow(NpgsqlConnection connection, Cliente cliente)
        {
            conn = connection;
            InitializeComponent();
            MostrarDetalhes(cliente);
        }

        private void MostrarDetalhes(Cliente cliente)
        {
            NomeClienteTxb.Text = cliente.Nome;
            CpfTxb.Text = cliente.Cpf;
            EmailTxb.Text = cliente.Email;
            DataNascimentoTxb.Text = cliente.DtNascimento.ToString();
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
            if (!string.IsNullOrEmpty(CpfTxb.Text))
            {
                if (long.TryParse(CpfTxb.Text, out long CPF) && CpfTxb.Text.Length == 11)
                {
                    string CPFFormatado = string.Format(@"{0:000\.000\.000\-00}", CPF);
                    CpfTxb.Text = CPFFormatado;
                }
                else
                {
                    MessageBox.Show("CPF inválido");
                }
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }

        private void FecharNovoModeloWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
