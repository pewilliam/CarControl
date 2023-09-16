using MahApps.Metro.Controls;
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
                    MessageBox.Show("Aluguel efetuado com sucesso!", "Aluguel concluído");
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
                e.Handled = true;
            
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

        private void ProcurarClienteBtn_Click(object sender, RoutedEventArgs e)
        {
            ProcurarClienteWindow procurarClienteWindow = new ProcurarClienteWindow(conn);
            procurarClienteWindow.Owner = null;
            if (procurarClienteWindow.ShowDialog() == true)
            {
                int selectedValue = procurarClienteWindow.ClienteId;
                IdClienteTxb.Text = selectedValue.ToString();
                IdModeloTxb.Focus();
                procurarClienteWindow.Close();
            }
        }

        private void ProcurarModeloBtn_Click(object sender, RoutedEventArgs e)
        {
            ProcurarModeloWindow procurarModeloWindow = new ProcurarModeloWindow(conn);
            procurarModeloWindow.Owner = null;
            if (procurarModeloWindow.ShowDialog() == true)
            {
                int selectedValue = procurarModeloWindow.ModeloId;
                IdModeloTxb.Text = selectedValue.ToString();
                IdFormaPagtoTxb.Focus();
            }
        }

        private void ProcurarFormaPagtoBtn_Click(object sender, RoutedEventArgs e)
        {
            ProcurarFormaPagto procurarFormaPagto = new ProcurarFormaPagto(conn);
            procurarFormaPagto.Owner = null;
            if (procurarFormaPagto.ShowDialog() == true)
            {
                int selectedValue = procurarFormaPagto.FormaPagtoId;
                IdFormaPagtoTxb.Text = selectedValue.ToString();
                DiasAluguelTxb.Focus();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
            
            if (Keyboard.IsKeyDown(Key.F4) && IdClienteTxb.IsFocused)
                ProcurarClienteBtn_Click(sender, e);
            
            if (Keyboard.IsKeyDown(Key.F4) && IdModeloTxb.IsFocused)
                ProcurarModeloBtn_Click(sender, e);
            
            if (Keyboard.IsKeyDown(Key.F4) && IdFormaPagtoTxb.IsFocused)
                ProcurarFormaPagtoBtn_Click(sender, e);
            
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SalvarNovoAluguelBtn_Click(sender, e);
            
        }

        private void DiasAluguelTxb_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
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
            else
                ValorAluguelTxb.Text = null;
            
        }
    }
}
 