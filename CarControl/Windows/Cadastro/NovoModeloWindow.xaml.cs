﻿using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace CarControl
{
    /// <summary>
    /// Lógica interna para NovoModeloWindow.xaml
    /// </summary>
    public partial class NovoModeloWindow : MetroWindow
    {
        NpgsqlConnection conn = new NpgsqlConnection();
        List<Fabricante> listFabricante = new List<Fabricante>();
        List<Categoria> listCategoria = new List<Categoria>();
        static int IdCarro = 0;

        public NovoModeloWindow(int idcarro, NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            IdCarro = idcarro;
            IdCarroTxb.Focus();
            PopulateFabricanteCB();
            PopulateCategoriaCB();
        }

        private void FecharNovoModeloWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SalvarNovoModeloBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCampos())
            {
                #region obtendo campos
                string nomeModelo = ModeloTxb.Text.ToUpper();
                string corModelo = CorTxb.Text.ToUpper();
                int qtdPortas = int.Parse(PortasTxb.Text);
                int qtdPassageiros = int.Parse(PassageirosTxb.Text);
                string combustivel = CombustívelTxb.Text.ToUpper();
                string placa = PlacaTxb.Text.ToUpper();
                string ano = AnoTxb.Text;
                string cambio = CambioTxb.Text.ToUpper();
                double preco = double.Parse(PrecoTxb.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                int IdCarro = int.Parse(IdCarroTxb.Text);
                int fabricante = (int)FabricanteCB.SelectedValue;
                int categoria = (int)CategoriaCB.SelectedValue;
                #endregion

                string sql = ($"INSERT INTO modelo(nome, cor, qtdportas, qtdpassageiros, combustivel, placa, ano, tipocambio, precodia, idcarro, idfabricante, idcategoria) " +
                    $"VALUES('{nomeModelo}', '{corModelo}', {qtdPortas}, {qtdPassageiros}, '{combustivel}', '{placa}', '{ano}', '{cambio}', {preco}, {IdCarro}, {fabricante}, {categoria});");

                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Modelo inserido com com sucesso!", "Modelo inserido");
                Close();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();

            if (e.Key == Key.Enter)
                SalvarNovoModeloBtn_Click(sender, e);

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

        private bool ValidarCampos()
        {
            #region validação
            if (string.IsNullOrEmpty(ModeloTxb.Text) ||
                string.IsNullOrEmpty(CorTxb.Text) ||
                string.IsNullOrEmpty(PortasTxb.Text) ||
                string.IsNullOrEmpty(PassageirosTxb.Text) ||
                string.IsNullOrEmpty(CombustívelTxb.Text) ||
                string.IsNullOrEmpty(PlacaTxb.Text) ||
                string.IsNullOrEmpty(AnoTxb.Text) ||
                string.IsNullOrEmpty(CambioTxb.Text) ||
                string.IsNullOrEmpty(PrecoTxb.Text) ||
                FabricanteCB.SelectedValue == null ||
                CategoriaCB.SelectedValue == null
                )
            #endregion
            {
                MessageBox.Show("Preencha todos os campos!", "Preenchimento");
                return false;
            }
            return true;
        }

        private void PopulateFabricanteCB()
        {
            listFabricante.Clear();
            string sql = "SELECT * FROM fabricante;";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Fabricante Fabricante = new(
                        reader.GetInt32(0),
                        reader.GetString(1)
                        );
                    listFabricante.Add(Fabricante);
                }
                reader.Close();
                FabricanteCB.ItemsSource = listFabricante;
                FabricanteCB.Items.Refresh();
            }
        }

        private void PopulateCategoriaCB()
        {
            listCategoria.Clear();
            string sql = $"SELECT * FROM categoria;";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Categoria categoria = new(
                        reader.GetInt32(0),
                        reader.GetString(1)
                        );
                    listCategoria.Add(categoria);
                }
                reader.Close();
                CategoriaCB.ItemsSource = listCategoria;
                CategoriaCB.Items.Refresh();
            }
        }

        private void PrecoTxb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PrecoTxb.Text != string.Empty)
            {
                double amount = double.Parse(PrecoTxb.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
                PrecoTxb.Text = amount.ToString();
                PrecoTxb.CaretIndex = PrecoTxb.Text.Length;
            }
        }

        private void PrecoTxb_LostFocus(object sender, RoutedEventArgs e)
        {
            double amount;
            PrecoTxb.Text = (double.TryParse(PrecoTxb.Text, out amount)) ? amount.ToString("C") : string.Empty;
        }

        private void NovoFabricanteBtn_Click(object sender, RoutedEventArgs e)
        {
            NovoFabricanteWindow novoFabricanteWindow = new NovoFabricanteWindow(conn);
            novoFabricanteWindow.ShowDialog();
            novoFabricanteWindow.Owner = this;
            PopulateFabricanteCB();
        }

        private void NovaCategoriaBtn_Click(object sender, RoutedEventArgs e)
        {
            NovaCategoriaWindow novaCategoriaWindow = new NovaCategoriaWindow(conn);
            novaCategoriaWindow.ShowDialog();
            novaCategoriaWindow.Owner = this;
            PopulateCategoriaCB();
        }

        private void IdCarroTxb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IdCarroTxb.Text != string.Empty)
            {
                int idCarro = int.Parse(IdCarroTxb.Text);
                string sql = $"SELECT nome FROM carro WHERE idcarro = {idCarro}";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        MessageBox.Show("Carro não existe! Cadastre um novo ou escolha algum existente.", "Carro não encontrado");
                        IdCarroTxb.Text = string.Empty;
                        NomeCarroTxb.Text = string.Empty;

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            IdCarroTxb.Focus();
                        }));
                    }
                    else
                    {
                        reader.Read(); // Ler a primeira (e única) linha
                        NomeCarroTxb.Text = reader.GetString(0);
                    }

                    reader.Close();
                }
            }
        }

        private void IdCarroTxb_Loaded(object sender, RoutedEventArgs e)
        {
            if (IdCarro != 0)
            {
                IdCarroTxb.Text = IdCarro.ToString();
                string sql = $"SELECT nome FROM carro WHERE idcarro = {IdCarro}";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        NomeCarroTxb.Text = reader.GetString(0);
                    }
                    reader.Close();
                }
                ModeloTxb.Focus();
            }
        }
    }
}
