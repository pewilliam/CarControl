using Npgsql;
using System.Windows;

namespace CarControl
{
    /// <summary>
    /// Lógica interna para NovoFabricanteWindow.xaml
    /// </summary>
    public partial class NovoFabricanteWindow : Window
    {
        //string connection = "Server=localhost;Port=5432;Database=base_carros;User id=postgres;Password=pedrow2001";
        string connection = "Server=localhost;Port=5433;Database=base_carros;User id=postgres;Password=pedrow2001";
        static NpgsqlConnection conn = new NpgsqlConnection();

        public NovoFabricanteWindow()
        {
            InitializeComponent();
            NomeFabricanteTxb.Focus();
        }

        private void SalvarCarroBtn_Click(object sender, RoutedEventArgs e)
        {
            string sql = ($"INSERT INTO carcontrol.fabricante(nome) VALUES ('{NomeFabricanteTxb.Text.ToUpper()}')");
            conn.ConnectionString = connection;
            conn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Fabricante inserido com sucesso!");
            Close();
        }

        private void FecharSaLVARCarroBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
