using Npgsql;
using System.Windows;

namespace CarControl
{
    /// <summary>
    /// Lógica interna para NovaCategoriaWindow.xaml
    /// </summary>
    public partial class NovaCategoriaWindow : Window
    {
        NpgsqlConnection conn = new NpgsqlConnection();

        public NovaCategoriaWindow(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            NomeCategoriaTxb.Focus();
        }

        private void SalvarCategoriaBtn_Click(object sender, RoutedEventArgs e)
        {
            string sql = ($"INSERT INTO carcontrol.categoria (nome) VALUES ('{NomeCategoriaTxb.Text.ToUpper()}');");
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
