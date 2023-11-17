using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para NovoRecebimentoWindow.xaml
    /// </summary>
    public partial class NovoRecebimentoWindow : MetroWindow
    {
        NpgsqlConnection conn = new();
        List<Recebimento> recebimentoList = new();

        public NovoRecebimentoWindow(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            MostrarRecebimentos();
        }

        private void MostrarRecebimentos()
        {
            dg.ItemsSource = null;
            recebimentoList.Clear();
            string sql = ($"SELECT idrecebimento, nome_cliente, valororiginal, valorrecebido, recebimento_dia_previsto, dhrecebimento, em_aberto FROM vw_recebimento WHERE em_aberto = TRUE ORDER BY idrecebimento;");

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    #region lendo alugueis
                    Recebimento recebimento = new(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetDouble(2),
                        reader.IsDBNull(3) ? null : reader.GetDouble(3),
                        DateOnly.FromDateTime(reader.GetDateTime(4)),
                        reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                        reader.GetBoolean(6)
                        );
                    #endregion
                    recebimentoList.Add(recebimento);
                }
                reader.Close();
                dg.ItemsSource = recebimentoList;
            }
        }

        private void MostrarDetalhes(int idRecebimento)
        {
            LimpaLabels();
            string sql = ($"SELECT * FROM vw_recebimento WHERE idrecebimento = {idRecebimento};");

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    idRecebimentoLabel.Content = idRecebimentoLabel.Content + reader.GetInt32(0).ToString();
                    idAluguelLabel.Content = idAluguelLabel.Content + reader.GetInt32(1).ToString();
                    dhAluguelLabel.Content = dhAluguelLabel.Content + reader.GetDateTime(2).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    idDevolucaoLabel.Content = idDevolucaoLabel.Content + (reader.IsDBNull(3) ? null : reader.GetInt32(3).ToString());
                    dhDevolucaoLabel.Content = dhDevolucaoLabel.Content + (reader.IsDBNull(3) ? null : reader.GetDateTime(4).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
                    idClienteLabel.Content = idClienteLabel.Content + reader.GetInt32(5).ToString();
                    nomeClienteLabel.Content = nomeClienteLabel.Content + reader.GetString(6);
                    idModeloLabel.Content = idModeloLabel.Content + reader.GetInt32(7).ToString();
                    nomeModeloLabel.Content = nomeModeloLabel.Content + reader.GetString(8);
                    valorOriginalLabel.Content = valorOriginalLabel.Content + reader.GetDouble(9).ToString("C");
                    valorRecebidoLabel.Content = valorRecebidoLabel.Content + (reader.IsDBNull(10) ? null : reader.GetDouble(10).ToString("C"));
                    dhRecebimentoLabel.Content = dhRecebimentoLabel.Content + (reader.IsDBNull(11) ? null : reader.GetDateTime(11).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
                    if (reader.GetBoolean(12))
                        emAbertoLabel.Content = emAbertoLabel.Content + "Sim";
                    else
                        emAbertoLabel.Content = emAbertoLabel.Content + "Não";
                    diaPrevistoRecebimentoLabel.Content = diaPrevistoRecebimentoLabel.Content + reader.GetDateTime(13).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                reader.Close();
            }
        }

        private void DataGridRow_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is Recebimento selectedItem)
                MostrarDetalhes(selectedItem.IdRecebimento);
            
        }

        private void LimpaLabels()
        {
            idRecebimentoLabel.Content = "Id Recebimento: ";
            idAluguelLabel.Content = "Id Aluguel: ";
            dhAluguelLabel.Content = "Data do Aluguel: ";
            idDevolucaoLabel.Content = "Id Devolução: ";
            dhDevolucaoLabel.Content = "Data da Devolução: ";
            idClienteLabel.Content = "Id Cliente: ";
            nomeClienteLabel.Content = "Cliente: ";
            idModeloLabel.Content = "Id Modelo: ";
            nomeModeloLabel.Content = "Modelo: ";
            emAbertoLabel.Content = "Em Aberto: ";
            valorOriginalLabel.Content = "Valor original: ";
            valorRecebidoLabel.Content = "Valor recebido: ";
            dhRecebimentoLabel.Content = "Data do recebimento: ";
            diaPrevistoRecebimentoLabel.Content = "Data prevista: ";
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                Close();
            
        }

        private void FecharRecebimentoWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ReceberBtn_Click(object sender, RoutedEventArgs e)
        {
            Recebimento r = dg.SelectedItem as Recebimento;
            if (r is not null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Deseja confirmar recebimento?", "Confirmar recebimento", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    double valorRecebido = r.ValorOriginal;
                    int idrecebimento = r.IdRecebimento;
                    var dhRecebimento = DateTime.Now;
                    string sql = $"UPDATE carcontrol.recebimento SET valorrecebido = {valorRecebido}, dhrecebimento = '{dhRecebimento.ToString("dd-MM-yyyy HH:mm:ss")}', em_aberto = FALSE WHERE idrecebimento = {idrecebimento};";

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Recebimento efetuado com sucesso!", "Recebimento concluído");
                    Close();
                }
                else
                    Close();
                
            }
        }
    }
}
