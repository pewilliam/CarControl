using CarControl.Models;
using Npgsql;
using System.Windows;

namespace CarControl
{
    /// <summary>
    /// Lógica interna para ModeloDetailsWindow.xaml
    /// </summary>
    public partial class ModeloDetailsWindow : Window
    {
        static NpgsqlConnection conn = new NpgsqlConnection();

        public ModeloDetailsWindow(Modelo modelo)
        {
            InitializeComponent();
            MostrarDetalhes(modelo.IdModelo);
        }

        private void MostrarDetalhes(int idModelo)
        {
            string connection = "Server=localhost;Port=5432;Database=base_carros;User id=postgres;Password=pedrow2001";
            conn.ConnectionString = connection;
            string sql = ($"SELECT * FROM carcontrol.vw_carro_modelo WHERE idmodelo = {idModelo};");

            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    idCarroLabel.Content = idCarroLabel.Content + reader.GetInt32(0).ToString();
                    idModeloLabel.Content = idModeloLabel.Content + reader.GetInt32(1).ToString();
                    carroNomeLabel.Content = reader.GetString(2);
                    modeloNomeLabel.Content = reader.GetString(3);
                    fabricanteNomeLabel.Content = reader.GetString(4);
                    categoriaNomeLabel.Content = reader.GetString(5);
                    corLabel.Content = reader.GetString(6);
                    portasLabel.Content = reader.GetInt32(7);
                    passageirosLabel.Content = reader.GetInt32(8);
                    combustivelLabel.Content = reader.GetString(9);
                    placaLabel.Content = reader.GetString(10);
                    anoLabel.Content = reader.GetString(11);
                    cambioLabel.Content = reader.GetString(12);
                    precoLabel.Content = reader.GetDecimal(13);
                }
                reader.Close();
                conn.Close();
            }
        }
    }
}
