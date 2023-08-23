using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

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
        static int IdCarro = -1;

        public NovoModeloWindow(int idcarro, NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            IdCarro = idcarro;
        }

        private void FecharNovoModeloWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SalvarNovoModeloBtn_Click(object sender, RoutedEventArgs e)
        {
            string nomeModelo = ModeloTxb.Text.ToUpper();
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

            string sql = ($"INSERT INTO carcontrol.modelo(nome, cor, qtdportas, qtdpassageiros, combustivel, placa, ano, tipocambio, preco, idcarro, idfabricante, idcategoria) " +
                $"VALUES('{nomeModelo}', '{corModelo}', {qtdPortas}, {qtdPassageiros}, '{combustivel}', '{placa}', '{ano}', '{cambio}', {preco}, {IdCarro}, {fabricante}, {categoria});");

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Modelo inserido com sucesso!");
            Close();
        }

        private void PopulateFabricanteCB(object sender, System.EventArgs e)
        {
            listFabricante.Clear();
            string sql = "SELECT * FROM carcontrol.fabricante;";

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
            string sql = $"SELECT * FROM carcontrol.categoria;";

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
        }

        private void NovaCategoriaBtn_Click(object sender, RoutedEventArgs e)
        {
            NovaCategoriaWindow novaCategoriaWindow = new NovaCategoriaWindow(conn);
            novaCategoriaWindow.ShowDialog();
            novaCategoriaWindow.Owner = this;
        }
    }
}
