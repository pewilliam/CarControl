using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Npgsql;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para NovoAluguelWindow.xaml
    /// </summary>
    public partial class NovoAluguelWindow : MetroWindow
    {
        NpgsqlConnection conn = new();

        public NovoAluguelWindow(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            IdClienteTxb.Focus();
        }

        private void SalvarNovoAluguelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCampo())
            {
                int idCliente = int.Parse(IdClienteTxb.Text);
                int idModelo = int.Parse(IdModeloTxb.Text);
                int idFormaPagto = int.Parse(IdFormaPagtoTxb.Text);
                int diasAluguel = int.Parse(DiasAluguelTxb.Text);

                string sql = $"INSERT INTO aluguel(idcliente, idmodelo, idformapagto, diasaluguel) VALUES ({idCliente}, {idModelo}, {idFormaPagto}, {diasAluguel})";

                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Devolução efetuada com sucesso!", "Devolução concluída");
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void IdClienteTxb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IdClienteTxb.Text != string.Empty)
            {
                int idCliente = int.Parse(IdClienteTxb.Text);
                string sql = $"SELECT nome FROM carcontrol.cliente WHERE idcliente = {idCliente}";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        MessageBox.Show("Cliente não existe! Cadastre um novo ou escolha algum existente.", "Cliente não encontrado");
                        IdClienteTxb.Text = string.Empty;
                        NomeClienteTxb.Text = string.Empty;

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            IdClienteTxb.Focus();
                        }));
                    }
                    else
                    {
                        reader.Read(); // Ler a primeira (e única) linha
                        NomeClienteTxb.Text = reader.GetString(0);
                    }

                    reader.Close();
                }
            }
        }

        private void IdModeloTxb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IdModeloTxb.Text != string.Empty)
            {
                int idModelo = int.Parse(IdModeloTxb.Text);
                string sql = $"SELECT nome FROM carcontrol.modelo WHERE idmodelo = {idModelo}";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        MessageBox.Show("Modelo não existe! Cadastre um novo ou escolha algum existente.", "Modelo não encontrado");
                        IdModeloTxb.Text = string.Empty;
                        NomeModeloTxb.Text = string.Empty;

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            IdModeloTxb.Focus();
                        }));
                    }
                    else
                    {
                        reader.Read(); // Ler a primeira (e única) linha
                        NomeModeloTxb.Text = reader.GetString(0);
                    }

                    reader.Close();
                }
            }
        }

        private void IdFormaPagtoTxb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IdFormaPagtoTxb.Text != string.Empty)
            {
                int idFormaPagto = int.Parse(IdFormaPagtoTxb.Text);
                string sql = $"SELECT nome FROM carcontrol.formapagto WHERE idformapagto = {idFormaPagto}";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        MessageBox.Show("Forma de pagamento não existe! Cadastre uma nova ou escolha alguma existente.", "Forma pagto. não encontrada");
                        IdFormaPagtoTxb.Text = string.Empty;
                        NomeFormaPagtoTxb.Text = string.Empty;

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            IdFormaPagtoTxb.Focus();
                        }));
                    }
                    else
                    {
                        reader.Read(); // Ler a primeira (e única) linha
                        NomeFormaPagtoTxb.Text = reader.GetString(0);
                    }

                    reader.Close();
                }
            }
        }

        private void DiasAluguelTxb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IdModeloTxb.Text != string.Empty && DiasAluguelTxb.Text != string.Empty)
            {
                int idModelo = int.Parse(IdModeloTxb.Text);
                string sql = $"SELECT precodia FROM carcontrol.modelo WHERE idmodelo = {idModelo}";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.HasRows)
                        {
                            MessageBox.Show("Cliente não existe! Cadastre um novo ou escolha algum existente.", "Cliente não encontrado");
                            IdClienteTxb.Focus();
                        }
                        else
                        {
                            double valorModelo = reader.GetDouble(0);
                            int diasAluguel = int.Parse(DiasAluguelTxb.Text);
                            ValorAluguelTxb.Text = (valorModelo * diasAluguel).ToString("C");
                        }
                    }
                    reader.Close();
                }
            }
        }

        private bool ValidarCampo()
        {
            if (string.IsNullOrEmpty(IdClienteTxb.Text))
            {
                MessageBox.Show("Preencha o código do cliente!", "Preenchimento");
                return false;
            }
            if (string.IsNullOrEmpty(IdModeloTxb.Text))
            {
                MessageBox.Show("Preencha o código do modelo!", "Preenchimento");
                return false;
            }
            if (string.IsNullOrEmpty(IdFormaPagtoTxb.Text))
            {
                MessageBox.Show("Preencha o código da forma de pagamento!", "Preenchimento");
                return false;
            }
            if (string.IsNullOrEmpty(DiasAluguelTxb.Text))
            {
                MessageBox.Show("Preencha a quantidade de dias que o modelo será alugado!", "Preenchimento");
                return false;
            }
            return true;
        }

        private void PreviewCharInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void PreviewNumberInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void FecharNovoAluguelWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
