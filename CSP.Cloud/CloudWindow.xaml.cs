using CSP.Core;
using CSP.Core.Model.Part;
using CSP.Core.Util;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CSP.Cloud
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class CloudWindow : Window
    {
        #region Fields

        private Ring ring = new Ring();
        private CloudStatus cloudStatus = CloudStatus.Stop;

        #endregion

        public CloudWindow()
        {
            InitializeComponent();
            Logger.Show = s => tbConsole.Text += s + "\r\n";
        }

        #region Events Button Click

        private void BtnServer_Click(object sender, RoutedEventArgs e)
        {
            int port = 1006;
            try {
                port = int.Parse(tbPort.Text);
            } catch (Exception ignore) { }

            switch (cloudStatus) {
                case CloudStatus.Start: {
                        cloudStatus = CloudStatus.Start;
                    ring.Close();
                } break;
                case CloudStatus.Stop: {
                    cloudStatus = CloudStatus.Stop;
                    ring.Create(port);
                } break;
            }
            ((Button) sender).Content = cloudStatus;
        }

        private void BtnServerInfo_Click(object sender, RoutedEventArgs e) => ring.NodeInfo();
        private void BtnExtendedInfo_Click(object sender, RoutedEventArgs e) => ring.NodeInfo(true);
        private void BtnCleanOutput_Click(object sender, RoutedEventArgs e) => tbConsole.Text = "";

        #endregion

    }
}
