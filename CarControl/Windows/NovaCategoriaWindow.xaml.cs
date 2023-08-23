using MahApps.Metro.Controls;
using Npgsql;
using System.Windows;

namespace CarControl
{
    /// <summary>
    /// Lógica interna para NovaCategoriaWindow.xaml
    /// </summary>
    public partial class NovaCategoriaWindow : MetroWindow
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
            if (ValidarCampo())
            {
                string sql = ($"INSERT INTO carcontrol.categoria (nome) VALUES ('{NomeCategoriaTxb.Text.ToUpper()}');");
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Categoria inserido com sucesso!");
                Close();
            }
        }

        private bool ValidarCampo()
        {
            if (string.IsNullOrEmpty(NomeCategoriaTxb.Text))
            {
                MessageBox.Show("Preencha o nome da categoria!", "Preenchimento");
                return false;
            }
            return true;
        }

        private void FecharSalvarCategoriaBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
