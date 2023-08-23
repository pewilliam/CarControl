﻿using Npgsql;
using System.Windows;

namespace CarControl
{
    /// <summary>
    /// Lógica interna para NovoFabricanteWindow.xaml
    /// </summary>
    public partial class NovoFabricanteWindow : Window
    {
        static NpgsqlConnection conn = new NpgsqlConnection();

        public NovoFabricanteWindow(NpgsqlConnection connection)
        {
            conn = connection;
            InitializeComponent();
            NomeFabricanteTxb.Focus();
        }

        private void SalvarCarroBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidarCampo())
            {
                string sql = ($"INSERT INTO carcontrol.fabricante(nome) VALUES ('{NomeFabricanteTxb.Text.ToUpper()}')");
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Fabricante inserido com sucesso!");
                Close();
            }
        }

        private bool ValidarCampo()
        {
            if (string.IsNullOrEmpty(NomeFabricanteTxb.Text))
            {
                MessageBox.Show("Preencha o nome do fabricante!", "Preenchimento");
                return false;
            }
            return true;
        }

        private void FecharSaLVARCarroBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
