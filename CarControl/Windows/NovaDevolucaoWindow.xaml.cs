﻿using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para NovaDevolucaoWindow.xaml
    /// </summary>
    public partial class NovaDevolucaoWindow : MetroWindow
    {
        NpgsqlConnection conn = new NpgsqlConnection();
        List<Aluguel> aluguelList = new List<Aluguel>();

        public NovaDevolucaoWindow(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            MostrarAlugueis();
        }

        private void MostrarAlugueis()
        {
            dg.ItemsSource = null;
            aluguelList.Clear();
            string sql = ($"SELECT * FROM carcontrol.aluguel WHERE em_andamento = TRUE ORDER BY idaluguel;");

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    #region lendo alugueis
                    Aluguel aluguel = new(
                        reader.GetInt32(0), //id
                        reader.GetInt32(1), //nome
                        reader.GetInt32(2), //cor
                        reader.GetInt32(3), //qtdportas
                        reader.GetDateTime(4), //qtdpassageiros
                        reader.GetInt32(5), //combustivel
                        reader.GetDouble(6),
                        reader.GetBoolean(7)
                        );
                    #endregion
                    aluguelList.Add(aluguel);
                }
                reader.Close();
                dg.ItemsSource = aluguelList;
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

        private void DataGridRow_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is Aluguel selectedItem)
            {
                MostrarDetalhes(selectedItem.IdAluguel);
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

        private void FecharNovaDevolucaoWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DevolverBtn_Click(object sender, RoutedEventArgs e)
        {
            Aluguel a = dg.SelectedItem as Aluguel;
            if (a is not null)
            {
                int idModelo = a.IdModelo;
                int idCliente = a.IdCliente;
                int idAluguel = a.IdAluguel;
                string sql = $"INSERT INTO carcontrol.devolucao(idmodelo, idcliente, idaluguel) VALUES ({idModelo}, {idCliente}, {idAluguel});";

                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Devolução efetuada com sucesso!");
                Close();
            }
        }
    }
}