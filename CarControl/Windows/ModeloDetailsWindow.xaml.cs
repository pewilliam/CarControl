using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;

namespace CarControl
{
    /// <summary>
    /// Lógica interna para ModeloDetailsWindow.xaml
    /// </summary>
    public partial class ModeloDetailsWindow : MetroWindow
    {
        static NpgsqlConnection conn = new NpgsqlConnection();
        List<Fabricante> listFabricante = new List<Fabricante>();
        List<Categoria> listCategoria = new List<Categoria>();
        int idModelo = 0;

        public ModeloDetailsWindow(Modelo modelo, NpgsqlConnection connection)
        {
            idModelo = modelo.IdModelo;
            conn = connection;
            InitializeComponent();
            MostrarDetalhes();
        }

        private void MostrarDetalhes()
        {
            double amount;
            string sql = $"SELECT nome, cor, qtdportas, qtdpassageiros, combustivel, placa, ano, tipocambio, preco, idfabricante, idcategoria " +
                $"FROM carcontrol.modelo " +
                $"WHERE idmodelo = {idModelo};";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    NomeModeloTxb.Text = reader.GetString(0);
                    CorTxb.Text = reader.GetString(1);
                    PortasTxb.Text = reader.GetInt32(2).ToString();
                    PassageirosTxb.Text = reader.GetInt32(3).ToString();
                    CombustívelTxb.Text = reader.GetString(4);
                    PlacaTxb.Text = reader.GetString(5);
                    AnoTxb.Text = reader.GetString(6);
                    CambioTxb.Text = reader.GetString(7);
                    PrecoTxb.Text = (double.TryParse(reader.GetDecimal(8).ToString(), out amount)) ? amount.ToString("C") : string.Empty; reader.GetDecimal(8).ToString();
                    FabricanteCB.SelectedValue = reader.GetInt32(9);
                    CategoriaCB.SelectedValue = reader.GetInt32(10);
                }
                reader.Close();
            }
        }

        private void PopulateFabricanteCB(object sender, System.EventArgs e)
        {
            listFabricante.Clear();
            string sql = "SELECT * FROM carcontrol.fabricante ORDER BY idfabricante;";

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

        private void PopulateCategoriaCB(object sender, System.EventArgs e)
        {
            listCategoria.Clear();
            string sql = $"SELECT * FROM carcontrol.categoria ORDER BY idcategoria;";

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

        private void NovoFabricanteBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NovoFabricanteWindow novoFabricanteWindow = new NovoFabricanteWindow(conn);
            novoFabricanteWindow.ShowDialog();
            novoFabricanteWindow.Owner = this;
        }

        private void NovaCategoriaBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NovaCategoriaWindow novaCategoriaWindow = new NovaCategoriaWindow(conn);
            novaCategoriaWindow.ShowDialog();
            novaCategoriaWindow.Owner = this;
        }

        private void SalvarNovoModeloBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            #region obter campos
            string nomeModelo = NomeModeloTxb.Text.ToUpper();
            string corModelo = CorTxb.Text.ToUpper();
            int qtdPortas = int.Parse(PortasTxb.Text);
            int qtdPassageiros = int.Parse(PassageirosTxb.Text);
            string combustivel = CombustívelTxb.Text.ToUpper();
            string placa = PlacaTxb.Text.ToUpper();
            string ano = AnoTxb.Text;
            string cambio = CambioTxb.Text.ToUpper();
            double preco = double.Parse(PrecoTxb.Text, NumberStyles.AllowCurrencySymbol | NumberStyles.Currency);
            int fabricante = (int)FabricanteCB.SelectedValue;
            int categoria = (int)CategoriaCB.SelectedValue;
            #endregion

            #region update query
            string sql = $"UPDATE carcontrol.modelo SET " +
                $"nome = '{nomeModelo}', " +
                $"cor = '{corModelo}', " +
                $"qtdportas = {qtdPortas}, " +
                $"qtdpassageiros = {qtdPassageiros}, " +
                $"combustivel = '{combustivel}', " +
                $"placa = '{placa}', " +
                $"ano = '{ano}', " +
                $"tipocambio = '{cambio}', " +
                $"preco = {preco}, " +
                $"idfabricante = {fabricante}, " +
                $"idcategoria = {categoria} " +
                $"WHERE idmodelo = {idModelo};";
            #endregion

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Modelo atualizado com sucesso!", "Concluído");
            Close();
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

        private void FecharNovoModeloWindowBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
    }
}
