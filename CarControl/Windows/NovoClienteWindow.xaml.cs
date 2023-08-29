using MahApps.Metro.Controls;
using Npgsql;
using System;
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
            if (CpfTxb.Text != string.Empty)
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

        private void SalvarNovoModeloBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCampo())
            {
                string sql = $"INSERT INTO carcontrol.cliente(nome, cpf, email, dtnascimento) VALUES (UPPER('{NomeClienteTxb.Text}'), '{CpfTxb.Text = new string(CpfTxb.Text.Where(char.IsDigit).ToArray())}', '{EmailTxb.Text}', '{DataNascimentoTxb.Text}')";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cliente inserido com sucesso!", "Sucesso!");
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
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

        private bool ValidarCampo()
        {
            if (string.IsNullOrEmpty(NomeClienteTxb.Text))
            {
                MessageBox.Show("Preencha o nome do cliente!", "Preenchimento");
                return false;
            }
            if (string.IsNullOrEmpty(CpfTxb.Text))
            {
                MessageBox.Show("Preencha o CPF do cliente!", "Preenchimento");
                return false;
            }
            if (string.IsNullOrEmpty(EmailTxb.Text))
            {
                MessageBox.Show("Preencha o e-mail do cliente!", "Preenchimento");
                return false;
            }
            if (string.IsNullOrEmpty(DataNascimentoTxb.Text))
            {
                MessageBox.Show("Preencha a data de nascimento do cliente!", "Preenchimento");
                return false;
            }
            return true;
        }

        private void DataNascimentoTxb_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DatePicker dt = (DatePicker)sender;
            string justNumbers = new String(dt.Text.Where(Char.IsDigit).ToArray());
            if (justNumbers.Length == 8)
            {
                string newDate = justNumbers.Insert(2, "/").Insert(5, "/");

                dt.SelectedDate = DateTime.Parse(newDate);
            }
        }
    }
}
