using CarControl.Models;
using CarControl.Windows;
using MahApps.Metro.Controls;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        LoginWindow loginwindow = new LoginWindow();
        NpgsqlConnection conn = new NpgsqlConnection();

        public MainWindow()
        {
            loginwindow.ShowDialog();
            conn = loginwindow.conn;
            if (conn.State == ConnectionState.Open)
            {
                InitializeComponent();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void MostrarCarroWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            CarrosWindow carrosWindow = new CarrosWindow(conn);
            carrosWindow.ShowDialog();
            carrosWindow.Owner = this;
        }

        private void ModelosCarroWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            ModelosWindow modelosWindow = new ModelosWindow(conn);
            modelosWindow.ShowDialog();
            modelosWindow.Owner = this;
        }
    }
}
