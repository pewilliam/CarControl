using CarControl.Windows;
using MahApps.Metro.Controls;
using Npgsql;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
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

        public MainWindow()
        {
            loginwindow.ShowDialog();
            conn = loginwindow.conn;
            if (conn.State == ConnectionState.Open)
            {
                IniciaRelogio();
                InitializeComponent();
                CurrentUserTxb.Text = "Usuário: " + conn.UserName.ToString();
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

        private void ClientesWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientesWindow clientesWindow = new ClientesWindow(conn);
            clientesWindow.ShowDialog();
            clientesWindow.Owner = this;
        }

        private void AluguelWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            AluguelWindow aluguelWindow = new AluguelWindow(conn);
            aluguelWindow.ShowDialog();
            aluguelWindow.Owner = this;
        }

        private void DevolucaoWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            DevolucoesWindow devolucoesWindow = new DevolucoesWindow(conn);
            devolucoesWindow.ShowDialog();
            devolucoesWindow.Owner = this;
        }

        private void RecebimentoWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            RecebimentosWindow recebimentosWindow = new RecebimentosWindow(conn);
            recebimentosWindow.ShowDialog();
            recebimentosWindow.Owner = this;
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

        private void FormasPagtoWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            FormasPagtoWindow formasPagtoWindow = new FormasPagtoWindow(conn);
            formasPagtoWindow.ShowDialog();
            formasPagtoWindow.Owner = this;
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

        private void TrocarUsuarioBtn_Click(object sender, RoutedEventArgs e)
        {
            conn.Close();
            LoginWindow loginwindow = new LoginWindow();
            loginwindow.ShowDialog();

            if (loginwindow.conn.UserName != null)
            {
                conn = loginwindow.conn;
                CurrentUserTxb.Text = "Usuário: " + conn.UserName.ToString();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Deseja encerrar a aplicação?", "Fechar aplicação", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                    Application.Current.Shutdown();

            }

            if (Keyboard.IsKeyDown(Key.C) && Keyboard.IsKeyDown(Key.LeftAlt))
                MostrarCarroWindowBtn_Click(sender, e);

            if (Keyboard.IsKeyDown(Key.M) && Keyboard.IsKeyDown(Key.LeftAlt))
                ModelosCarroWindowBtn_Click(sender, e);

            if (Keyboard.IsKeyDown(Key.L) && Keyboard.IsKeyDown(Key.LeftAlt))
                ClientesWindowBtn_Click(sender, e);

            if (Keyboard.IsKeyDown(Key.A) && Keyboard.IsKeyDown(Key.LeftAlt))
                AluguelWindowBtn_Click(sender, e);

            if (Keyboard.IsKeyDown(Key.D) && Keyboard.IsKeyDown(Key.LeftAlt))
                DevolucaoWindowBtn_Click(sender, e);

            if (Keyboard.IsKeyDown(Key.R) && Keyboard.IsKeyDown(Key.LeftAlt))
                RecebimentoWindowBtn_Click(sender, e);

            if (Keyboard.IsKeyDown(Key.F) && Keyboard.IsKeyDown(Key.LeftAlt))
                FabricantesCarroWindowBtn_Click(sender, e);

            if (Keyboard.IsKeyDown(Key.T) && Keyboard.IsKeyDown(Key.LeftAlt))
                CategoriasCarroWindowBtn_Click(sender, e);

            if (Keyboard.IsKeyDown(Key.O) && Keyboard.IsKeyDown(Key.LeftAlt))
                FormasPagtoWindowBtn_Click(sender, e);

        }

        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
            CenterWindowOnScreen();
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = Width;
            double windowHeight = Height;
            Left = (screenWidth / 2) - (windowWidth / 2);
            Top = (screenHeight / 2) - (windowHeight / 2);
        }
    }
}
