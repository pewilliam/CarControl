using Npgsql;
using System.Windows;

namespace CarControl
{
    /// <summary>
    /// Lógica interna para NovaCategoriaWindow.xaml
    /// </summary>
    public partial class NovaCategoriaWindow : Window
    {
        //string connection = "Server=localhost;Port=5432;Database=base_carros;User id=postgres;Password=pedrow2001";
        string connection = "Server=localhost;Port=5433;Database=base_carros;User id=postgres;Password=pedrow2001";
        NpgsqlConnection conn = new NpgsqlConnection();

        public NovaCategoriaWindow()
        {
            InitializeComponent();
            NomeCategoriaTxb.Focus();
        }

        private void SalvarCategoriaBtn_Click(object sender, RoutedEventArgs e)
        {
            string sql = ($"INSERT INTO carcontrol.categoria (nome) VALUES ('{NomeCategoriaTxb.Text.ToUpper()}');");
            conn.ConnectionString = connection;
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Categoria inserido com sucesso!");
            Close();
        }

        private void FecharSalvarCategoriaBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
