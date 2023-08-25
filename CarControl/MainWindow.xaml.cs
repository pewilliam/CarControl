using CarControl.Windows;
using MahApps.Metro.Controls;
using Npgsql;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CarControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        LoginWindow loginwindow = new LoginWindow();
        NpgsqlConnection conn = new NpgsqlConnection();
        Random random = new();

        public MainWindow()
        {
            loginwindow.ShowDialog();
            conn = loginwindow.conn;
            if (conn.State == ConnectionState.Open)
            {
                IniciaRelogio();
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

        private void FabricantesCarroWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            FabricantesWindow fabricantesWindow = new FabricantesWindow(conn);
            fabricantesWindow.ShowDialog();
            fabricantesWindow.Owner = this;
        }

        private void CategoriasCarroWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            CategoriasWindows categoriasWindows = new CategoriasWindows(conn);
            categoriasWindows.ShowDialog();
            categoriasWindows.Owner = this;
        }

        private void IniciaRelogio()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Tickevent;
            timer.Start();
        }

        private void Tickevent(object sender, EventArgs e)
        {
            ClockLabel.Text = DateTime.Now.ToString();
        }

        private void UsuariosWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            UsuariosWindow usuariosWindow = new(conn);
            usuariosWindow.ShowDialog();
            usuariosWindow.Owner = this;
        }
    }
}
