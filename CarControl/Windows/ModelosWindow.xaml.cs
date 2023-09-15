using CarControl.Models;
using MahApps.Metro.Controls;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CarControl
{
    /// <summary>
    /// Lógica interna para ModelosWindow.xaml
    /// </summary>
    public partial class ModelosWindow : MetroWindow
    {
        static NpgsqlConnection conn = new NpgsqlConnection();
        static List<Modelo> modeloList = new List<Modelo>();
        List<Carro> carroList = new List<Carro>();

        public ModelosWindow(NpgsqlConnection connection)
        {
            InitializeComponent();
            conn = connection;
            PopulateCarrosCB();
            SearchModeloTxb.Focus();
            MostrarModelos();
        }

        private void MostrarModelos()
        {
            dg.ItemsSource = null;
            modeloList.Clear();
            string sql = ($"SELECT * FROM carcontrol.modelo ORDER BY idmodelo;");

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    #region lendo modelos
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
                        reader.GetInt32(12), //idfabricante
                        reader.GetBoolean(13) //idfabricante
                        );
                    #endregion
                    modeloList.Add(modelo);
                }
                reader.Close();
                dg.ItemsSource = modeloList;
            }
        }

        private void MostrarModelos(int idcarro)
        {
            dg.ItemsSource = null;
            modeloList.Clear();
            string sql = ($"SELECT * FROM carcontrol.modelo WHERE idcarro = {idcarro} ORDER BY idmodelo;");

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    #region lendo modelos
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
                        reader.GetInt32(12),
                        reader.GetBoolean(13)//idfabricante
                        );
                    #endregion
                    modeloList.Add(modelo);
                }
                reader.Close();
                dg.ItemsSource = modeloList;
            }
        }

        private void MostrarDetalhes(int idModelo)
        {
            LimpaLabels();
            string sql = ($"SELECT * FROM carcontrol.vw_carro_modelo WHERE idmodelo = {idModelo};");

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
                    precoLabel.Content = precoLabel.Content + reader.GetDecimal(13).ToString("C");
                    if(reader.GetBoolean(14) == true)
                        disponivelLabel.Content = disponivelLabel.Content + "Sim";
                    else
                        disponivelLabel.Content = disponivelLabel.Content + "Não";
                }
                reader.Close();
            }
        }

        private void PopulateCarrosCB()
        {
            carroList.Clear();
            string sql = "SELECT * FROM carcontrol.carro;";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Carro carro = new(
                        reader.GetInt32(0),
                        reader.GetString(1)
                        );
                    carroList.Add(carro);
                }
                reader.Close();
                CarrosCB.ItemsSource = carroList;
                CarrosCB.Items.Refresh();
            }
        }

        private void LimpaLabels()
        {
            #region limpar labels
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
            disponivelLabel.Content = "Disponível: ";
            #endregion
        }

        private void DataGridRow_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is Modelo selectedItem)
            {
                ModeloDetailsWindow modeloDetailsWindow = new ModeloDetailsWindow(selectedItem, conn);
                modeloDetailsWindow.ShowDialog();
                modeloDetailsWindow.Owner = this;
                AtualizaDataGrid();
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
            if(CarrosCB.SelectedValue != null)
            {
                int idCarro = (int)CarrosCB.SelectedValue;
                NovoModeloWindow novoModeloWindow = new NovoModeloWindow(idCarro, conn);
                novoModeloWindow.ShowDialog();
                novoModeloWindow.Owner = this;
            }
            else
            {
                NovoModeloWindow novoModeloWindow = new NovoModeloWindow(0, conn);
                novoModeloWindow.ShowDialog();
                novoModeloWindow.Owner = this;
            }
            AtualizaDataGrid();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            if (Keyboard.IsKeyDown(Key.N) && (Keyboard.IsKeyDown(Key.LeftAlt)))
            {
                NovoModeloBtn_Click(sender, e);
            }
        }

        private void SearchModeloTxb_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txb = sender as TextBox;
            if (txb.Text == null)
            {
                MostrarModelos((int)CarrosCB.SelectedValue);
            }
            else
            {
                var filteredList = modeloList.Where(x => x.Nome.Contains(txb.Text));
                dg.ItemsSource = null;
                dg.ItemsSource = filteredList;
            }
        }

        private void AbrirModeloBtn_Click(object sender, RoutedEventArgs e)
        {
            Modelo m = dg.SelectedItem as Modelo;
            if (m is not null)
            {
                ModeloDetailsWindow modeloDetailsWindow = new ModeloDetailsWindow(m, conn);
                modeloDetailsWindow.ShowDialog();
                modeloDetailsWindow.Owner = this;
                AtualizaDataGrid();
            }
        }

        private void CarrosCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CarrosCB.SelectedValue == null)
            {
                MostrarModelos();
            }
            else
            {
                MostrarModelos((int)CarrosCB.SelectedValue);
                SearchModeloTxb.Text = "";
            }
        }

        private void ClearCarrosCBBtn_Click(object sender, RoutedEventArgs e)
        {
            CarrosCB.SelectedValue = null;
            SearchModeloTxb.Text = "";
        }

        private void AtualizaDataGrid()
        {
            if (CarrosCB.SelectedValue == null)
            {
                MostrarModelos();
            }
            else
            {
                MostrarModelos((int)CarrosCB.SelectedValue);
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AbrirModeloBtn_Click(sender, e); // Isso impede que o evento "Enter" seja processado
            }
        }
    }
}
