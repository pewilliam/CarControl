using CarControl.Models;
using Npgsql;
using MahApps.Metro.Controls;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para ClienteDetailsWindow.xaml
    /// </summary>
    public partial class ClienteDetailsWindow : MetroWindow
    {
        private string previousCpf = "";
        private string previousDate = "";
        NpgsqlConnection conn = new NpgsqlConnection();
        Cliente cliente;

        public ClienteDetailsWindow(NpgsqlConnection connection, Cliente client)
        {
            conn = connection;
            cliente = client;
            InitializeComponent();
            MostrarDetalhes(cliente);
        }

        private void MostrarDetalhes(Cliente cliente)
        {
            NomeClienteTxb.Text = cliente.Nome;
            CpfTxb.Text = cliente.Cpf;
            EmailTxb.Text = cliente.Email;
            DataNascimentoTextBox.Text = cliente.DtNascimento.ToString("dd/MM/yyyy");
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                AtualizarClienteBtn_Click(sender, e);
            }
        }

        private void FecharClienteWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
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

        private void AtualizarClienteBtn_Click(object sender, RoutedEventArgs e)
        {
            string sql = $"UPDATE cliente SET cpf = '{CpfTxb.Text = new string(CpfTxb.Text.Where(char.IsDigit).ToArray())}', email = '{EmailTxb.Text}', dtnascimento = '{DataNascimentoTextBox.Text}' WHERE idcliente = {cliente.IdCliente};";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Cliente atualizado com sucesso!", "Concluído");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
        }
    }
}
