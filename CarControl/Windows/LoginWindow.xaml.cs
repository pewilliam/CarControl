using System;
using MahApps.Metro.Controls;
using System.IO;
using Npgsql;
using System.Windows;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : MetroWindow
    {
        static IniFile ini = new IniFile();
        public NpgsqlConnection conn = new NpgsqlConnection();

        public LoginWindow()
        {
            InitializeComponent();
            txbUser.Focus();
        }

        private void ConnectionDB(string login, string password)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CarControl.ini");
            if (!File.Exists(path))
            {
                ini.Write("ip", "");
                ini.Write("port", "");
                ini.Write("base", "");
            }
            var ip = ini.Read("ip");
            var port = ini.Read("port");
            var db = ini.Read("base");

            string connection = ($"Server={ip}; Port={port}; Database={db}; User Id={login}; Password={password};");

            conn.ConnectionString = connection;

            conn.Open();
        }

        private void btnSalvar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                ConnectionDB(txbUser.Text, txbPassword.Password);
                Close();
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("authentication"))
                    MessageBox.Show("Usuário e/ou senha inválido(s)!");
                else
                {
                    MessageBox.Show("Verifique os dados do ini!");

                    IniConfigWindow iniConfigWindow = new IniConfigWindow();
                    iniConfigWindow.ShowDialog();
                    iniConfigWindow.Owner = this;
                }
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnSalvar_Click(sender, e);
            }
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
