using CarControl.Models;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Controls;

namespace CarControl
{
    /// <summary>
    /// Lógica interna para ModelosWindow.xaml
    /// </summary>
    public partial class ModelosWindow : Window
    {
        static NpgsqlConnection conn = new NpgsqlConnection();
        static List<Modelo> modeloList = new List<Modelo>();
        static int IdCarro = -1;

        public ModelosWindow(int idcarro)
        {
            InitializeComponent();
            IdCarro = idcarro;
            MostrarModelos(IdCarro);
        }

        private void MostrarModelos(int idcarro)
        {
            modeloList.Clear();
            string connection = "Server=localhost;Port=5433;Database=base_carros;User id=postgres;Password=pedrow2001";
            conn.ConnectionString = connection;
            string sql = ($"SELECT * FROM carcontrol.modelo WHERE idcarro = {idcarro};");

            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Modelo modelo = new(
                        reader.GetInt32(0), //id
                        reader.GetString(1), //nome
                        reader.GetString(2), //cor
                        reader.GetInt32(3), //qtdportas
                        reader.GetInt32(4), //qtdpassageiros
                        reader.GetString(5), //combustivel
                        reader.GetString(6), //placa
                        reader.GetString(7), //ano
                        reader.GetString(8), //tipocambio
                        reader.GetDouble(9), //preco
                        reader.GetInt32(10), //idcarro
                        reader.GetInt32(11), //idfabricante
                        reader.GetInt32(12) //idfabricante
                        );
                    modeloList.Add(modelo);
                }
                reader.Close();
                conn.Close();
                dg.ItemsSource = modeloList;
            }
        }

        private void MostrarDetalhes(int idModelo)
        {
            LimpaLabels();
            string connection = "Server=localhost;Port=5433;Database=base_carros;User id=postgres;Password=pedrow2001";
            conn.ConnectionString = connection;
            string sql = ($"SELECT * FROM carcontrol.vw_carro_modelo WHERE idmodelo = {idModelo};");

            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    idModeloLabel.Content = idModeloLabel.Content + reader.GetInt32(0).ToString();
                    idCarroLabel.Content = idCarroLabel.Content + reader.GetInt32(1).ToString();
                    carroLabel.Content = carroLabel.Content + reader.GetString(2);
                    modeloLabel.Content = modeloLabel.Content + reader.GetString(3);
                    fabricanteLabel.Content = fabricanteLabel.Content + reader.GetString(4);
                    categoriaLabel.Content = categoriaLabel.Content + reader.GetString(5);
                    corLabel.Content = corLabel.Content + reader.GetString(6);
                    portasLabel.Content = portasLabel.Content + reader.GetInt32(7).ToString();
                    passageirosLabel.Content = passageirosLabel.Content + reader.GetInt32(8).ToString();
                    combustivelLabel.Content = combustivelLabel.Content + reader.GetString(9);
                    placaLabel.Content = placaLabel.Content + reader.GetString(10);
                    anoLabel.Content = anoLabel.Content + reader.GetString(11);
                    cambioLabel.Content = cambioLabel.Content + reader.GetString(12);
                    precoLabel.Content = precoLabel.Content + "R$ " + reader.GetDecimal(13).ToString();
                }
                reader.Close();
                conn.Close();
            }
        }

        private void LimpaLabels()
        {
            idCarroLabel.Content = "Id carro: ";
            idModeloLabel.Content = "Id modelo: ";
            carroLabel.Content = "Carro: ";
            modeloLabel.Content = "Modelo: ";
            fabricanteLabel.Content = "Fabricante: ";
            categoriaLabel.Content = "Categoria: ";
            corLabel.Content = "Cor: ";
            portasLabel.Content = "Portas: ";
            passageirosLabel.Content = "Passageiros: ";
            combustivelLabel.Content = "Combustível: ";
            placaLabel.Content = "Placa: ";
            anoLabel.Content = "Ano: ";
            cambioLabel.Content = "Câmbio: ";
            precoLabel.Content = "Preço: ";
        }

        private void DataGridRow_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is Modelo selectedItem)
            {
                ModeloDetailsWindow modeloDetailsWindow = new ModeloDetailsWindow(selectedItem);
                modeloDetailsWindow.ShowDialog();
                modeloDetailsWindow.Owner = this;
            }
        }

        private void DataGridRow_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is Modelo selectedItem)
            {
                MostrarDetalhes(selectedItem.IdModelo);
            }
        }

        private void FecharModeloWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NovoModeloBtn_Click(object sender, RoutedEventArgs e)
        {
            NovoModeloWindow novoModeloWindow = new NovoModeloWindow(IdCarro);
            novoModeloWindow.ShowDialog();
            MostrarModelos(IdCarro);
        }

        private void SearchModeloTxb_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txb = sender as TextBox;
            if (txb.Text == null)
            {
                MostrarModelos(IdCarro);
            }
            else
            {
                var filteredList = modeloList.Where(x => x.Nome.ToLower().Contains(txb.Text.ToLower()));
                dg.ItemsSource = null;
                MostrarModelos(IdCarro);
                dg.ItemsSource = filteredList;
            }
        }
    }
}
