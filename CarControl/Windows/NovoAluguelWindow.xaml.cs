using MahApps.Metro.Controls;
using Npgsql;
using System;
using System.Windows;

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
        }

        private void SalvarNovoAluguelBtn_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Aluguel feito com sucesso!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                    while (reader.Read())
                    {
                        if (reader.GetString(0) == null)
                        {
                            MessageBox.Show("Cliente não existe! Cadastre um novo ou escolha algum existente.", "Cliente não encontrado");
                            IdClienteTxb.Focus();
                        }
                        else
                        {
                            NomeClienteTxb.Text = reader.GetString(0);
                        }
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
                    while (reader.Read())
                    {
                        if (reader.GetString(0) == null)
                        {
                            MessageBox.Show("Modelo não existe! Cadastre um novo ou escolha algum existente.", "Modelo não encontrado");
                            IdModeloTxb.Focus();
                        }
                        else
                        {
                            NomeModeloTxb.Text = reader.GetString(0);
                        }
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
                    while (reader.Read())
                    {
                        if (reader.GetString(0) == null)
                        {
                            MessageBox.Show("Cliente não existe! Cadastre um novo ou escolha algum existente.", "Cliente não encontrado");
                            IdFormaPagtoTxb.Focus();
                        }
                        else
                        {
                            NomeFormaPagtoTxb.Text = reader.GetString(0);
                        }
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
                        if (reader.GetDecimal(0) == null)
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

        private void FecharNovoAluguelWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
