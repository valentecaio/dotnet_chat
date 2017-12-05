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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Interface;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variables
        
        private chatClient client;
        
        #endregion

        #region init

        public MainWindow()
        {
            InitializeComponent();

            this.client = new chatClient();
        }

        #endregion

        #region callbacks

        private void btSend_Click(object sender, RoutedEventArgs e)
        {
            this.client.sendMessage(this.tbSend.Text);
        }

        private void btConfApply_Click(object sender, RoutedEventArgs e)
        {
            this.client.name = this.tbUsername.Text;
            this.client.serverPort = this.tbServerPort.Text;
            this.client.serverHost = this.tbServerIP.Text;
        }

        private void btConfReset_Click(object sender, RoutedEventArgs e)
        {
            this.tbUsername.Text = this.client.name;
            this.tbServerPort.Text = this.client.serverPort;
            this.tbServerIP.Text = this.client.serverHost;
        }

        private void menuLogin_Click(object sender, RoutedEventArgs e)
        {
            this.client.connect();
        }

        private void menuLogout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuQuit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuTestServer_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion callback
    }
}
