using MahApps.Metro.Controls;
using System.Windows;

namespace CarControl.Windows
{
    /// <summary>
    /// Lógica interna para IniConfigWindow.xaml
    /// </summary>
    public partial class IniConfigWindow : MetroWindow
    {
        IniFile ini = new IniFile();

        public IniConfigWindow()
        {
            InitializeComponent();
            FillFields();
            txbIP.Focus();
        }

        private void FillFields()
        {
            txbIP.Text = ini.Read("ip");
            txbPort.Text = ini.Read("port");
            txbBase.Text = ini.Read("base");
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            ini.Write("ip", txbIP.Text);
            ini.Write("port", txbPort.Text);
            ini.Write("base", txbBase.Text);

            MessageBox.Show("Configuração salva com sucesso!", "Concluído");
            Close();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
