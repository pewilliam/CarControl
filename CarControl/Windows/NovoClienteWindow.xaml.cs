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
        private string previousCpf = "";
        private string previousDate = "";
        NpgsqlConnection conn = new NpgsqlConnection();

        public NovoClienteWindow(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            NomeClienteTxb.Focus();
        }

        public string FormatCPF(string sender, string previousText)
        {
            string response = sender.Trim();
            if (response != string.Empty)
            {
                if (response.Length == 3 && previousText.Length < 3)
                    response = response.Insert(3, ".");
                if (response.Length == 7 && previousText.Length < 7)
                    response = response.Insert(7, ".");
                if (response.Length == 11 && previousText.Length < 11)
                    response = response.Insert(11, "-");
            }
            return response;
        }

        public string FormatDate(string sender, string previousText)
        {
            string response = sender.Trim();
            if (response != string.Empty)
            {
                if (response.Length == 2 && previousText.Length < 2)
                    response = response.Insert(2, "/");
                if (response.Length == 5 && previousText.Length < 5)
                    response = response.Insert(5, "/");
            }
            return response;
        }


        private void SalvarNovoClienteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCampo())
            {
                string nomeCliente = NomeClienteTxb.Text.Trim();
                string cpfCliente = new string(CpfTxb.Text.Where(char.IsDigit).ToArray());
                string emailCliente = EmailTxb.Text.Trim();
                string dtNascimento = DataNascimentoTextBox.Text;

                string sql = $"INSERT INTO carcontrol.cliente(nome, cpf, email, dtnascimento) VALUES (UPPER('{nomeCliente}'), '{cpfCliente}', '{emailCliente}', '{dtNascimento}')";
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

        private void FecharNovoClienteWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SalvarNovoClienteBtn_Click (sender, e);
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
            if (string.IsNullOrEmpty(DataNascimentoTextBox.Text))
            {
                MessageBox.Show("Preencha a data de nascimento do cliente!", "Preenchimento");
                return false;
            }
            return true;
        }

        private void CpfTxb_TextChanged(object sender, TextChangedEventArgs e)
        {
            string currentText = CpfTxb.Text;
            CpfTxb.Text = FormatCPF(currentText, previousCpf);
            CpfTxb.CaretIndex = CpfTxb.Text.Length;

            previousCpf = currentText;
        }

        private void DataNascimentoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string currentText = DataNascimentoTextBox.Text;
            DataNascimentoTextBox.Text = FormatDate(currentText, previousDate);
            DataNascimentoTextBox.CaretIndex = DataNascimentoTextBox.Text.Length;

            previousDate = currentText;
        }
    }
}
