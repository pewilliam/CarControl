using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para DevolucoesWindow.xaml
    /// </summary>
    public partial class DevolucoesWindow : MetroWindow
    {
        NpgsqlConnection conn = new NpgsqlConnection();
        List<Devolucao> devolucaoList = new List<Devolucao>();

        public DevolucoesWindow(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            MostrarDevolucoes();
        }

        private void MostrarDevolucoes()
        {
            dg.ItemsSource = null;
            devolucaoList.Clear();
            string sql = ($"SELECT * FROM vw_devolucao ORDER BY iddevolucao");

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    #region lendo devoluções
                    Devolucao devolucao = new(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetString(2),
                        reader.GetInt32(3),
                        reader.GetString(4),
                        reader.GetInt32(5),
                        reader.GetDateTime(6)
                        );
                    #endregion
                    devolucaoList.Add(devolucao);
                }
                reader.Close();
                dg.ItemsSource = devolucaoList;
            }
        }

        private void DataGridRow_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is Devolucao selectedItem)
            {
                MostrarDetalhes(selectedItem.IdAluguel);
            }
        }

        private void MostrarDetalhes(int idAluguel)
        {
            LimpaLabels();
            string sql = ($"SELECT * FROM carcontrol.vw_aluguel WHERE idaluguel = {idAluguel};");

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    idAluguelLabel.Content = idAluguelLabel.Content + reader.GetInt32(0).ToString();
                    idClienteLabel.Content = idClienteLabel.Content + reader.GetInt32(1).ToString();
                    clienteLabel.Content = clienteLabel.Content + reader.GetString(2);
                    idModeloLabel.Content = idModeloLabel.Content + reader.GetInt32(3).ToString();
                    modeloLabel.Content = modeloLabel.Content + reader.GetString(4);
                    idFormaPagtoLabel.Content = idFormaPagtoLabel.Content + reader.GetInt32(5).ToString();
                    formaPagtoLabel.Content = formaPagtoLabel.Content + reader.GetString(6);
                    dataAluguelLabel.Content = dataAluguelLabel.Content + reader.GetDateTime(7).ToString();
                    diasLabel.Content = diasLabel.Content + reader.GetInt32(8).ToString();
                    valorTotalLabel.Content = valorTotalLabel.Content + reader.GetDecimal(9).ToString("C");
                }
                reader.Close();
            }
        }

        private void LimpaLabels()
        {
            idAluguelLabel.Content = "Id aluguel: ";
            idClienteLabel.Content = "Id cliente: ";
            clienteLabel.Content = "Cliente: ";
            idModeloLabel.Content = "Id modelo: ";
            modeloLabel.Content = "Modelo: ";
            idFormaPagtoLabel.Content = "Id. forma pagto: ";
            formaPagtoLabel.Content = "Forma pagto: ";
            dataAluguelLabel.Content = "Data do aluguel: ";
            diasLabel.Content = "Dias aluguel: ";
            valorTotalLabel.Content = "Valor total: ";
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
            if (Keyboard.IsKeyDown(Key.N) && (Keyboard.IsKeyDown(Key.LeftAlt)))
            {
                NovaDevolucaoBtn_Click(sender, e);
            }
        }

        private void FecharDevolucaoWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NovaDevolucaoBtn_Click(object sender, RoutedEventArgs e)
        {
            NovaDevolucaoWindow novaDevolucaoWindow = new NovaDevolucaoWindow(conn);
            novaDevolucaoWindow.ShowDialog();
            novaDevolucaoWindow.Owner = this;
            MostrarDevolucoes();
        }
    }
}
