using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

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
            MostrarDetalhes(modelo);
        }

        private void MostrarDetalhes(Modelo modelo)
        {
            PopulateFabricanteCB();
            PopulateCategoriaCB();

            double amount;

            NomeModeloTxb.Text = modelo.Nome;
            CorTxb.Text = modelo.Cor;
            PortasTxb.Text = modelo.QtdPortas.ToString();
            PassageirosTxb.Text = modelo.QtdPassageiros.ToString();
            CombustívelTxb.Text = modelo.Combustivel;
            PlacaTxb.Text = modelo.Placa;
            AnoTxb.Text = modelo.Ano;
            CambioTxb.Text = modelo.TipoCambio;
            PrecoTxb.Text = (double.TryParse(modelo.Preco.ToString(), out amount)) ? amount.ToString("C") : string.Empty; modelo.Preco.ToString();
            FabricanteCB.SelectedValue = modelo.IdFabricante;
            CategoriaCB.SelectedValue = modelo.IdCategoria;
        }

        private void PopulateFabricanteCB()
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

        private void PopulateCategoriaCB()
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
