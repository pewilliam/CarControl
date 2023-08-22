using CarControl.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CarControl
{
    /// <summary>
    /// Lógica interna para NovoCarroWindow.xaml
    /// </summary>
    public partial class NovoCarroWindow : Window
    {
        static NpgsqlConnection conn = new NpgsqlConnection();

        public NovoCarroWindow()
        {
            InitializeComponent();
            NomeCarroTxb.Focus();
        }

        private void FecharSaLVARCarroBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SalvarCarroBtn_Click(object sender, RoutedEventArgs e)
        {
            string connection = "Server=localhost;Port=5433;Database=base_carros;User id=postgres;Password=pedrow2001";
            conn.ConnectionString = connection;
            string sql = ($"INSERT INTO carcontrol.carro(nome) VALUES('{NomeCarroTxb.Text.ToUpper()}');");

            conn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Carro inserido com sucesso!");
            conn.Close();
            Close();
        }
    }
}
