﻿using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            string sql = ($"SELECT idaluguel, idcliente, nome_cliente, idmodelo, nome_modelo, idformapagto, forma_pagto, dhaluguel, diasaluguel, valoraluguel, em_andamento FROM vw_aluguel WHERE em_andamento = TRUE ORDER BY idaluguel;");

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    #region lendo alugueis
                    Aluguel aluguel = new(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetString(2),
                        reader.GetInt32(3),
                        reader.GetString(4),
                        reader.GetInt32(5),
                        reader.GetString(6),
                        reader.GetDateTime(7),
                        reader.GetInt32(8),
                        reader.GetDouble(9),
                        reader.GetBoolean(10)
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
            string sql = ($"SELECT * FROM vw_aluguel WHERE idaluguel = {idAluguel};");

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
                MostrarDetalhes(selectedItem.IdAluguel);
            
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
                MessageBoxResult messageBoxResult = MessageBox.Show("Deseja confirmar devolução?", "Confirmar devolução", MessageBoxButton.YesNo);
                if(messageBoxResult == MessageBoxResult.Yes)
                {
                    int idModelo = a.IdModelo;
                    int idCliente = a.IdCliente;
                    int idAluguel = a.IdAluguel;
                    string sql = $"INSERT INTO devolucao(idmodelo, idcliente, idaluguel) VALUES ({idModelo}, {idCliente}, {idAluguel});";

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Devolução efetuada com sucesso!", "Devolução concluída");
                    Close();
                }
                else
                    Close();
                
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
            
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                DevolverBtn_Click(sender, e); // Isso impede que o evento "Enter" seja processado
            
        }
    }
}
