using CSP.Core.Model.Part;
using System;
using System.Windows;

namespace CSP.Cloud
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        ServerStatus serverStatus = ServerStatus.Stop;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Evants Button Click

        private void btnServer_Click(object sender, RoutedEventArgs e)
        {
            switch (serverStatus) {
                case ServerStatus.Start: {
                    serverStatus = ServerStatus.Stop;

                } break;
                case ServerStatus.Stop: {
                    serverStatus = ServerStatus.Start;

                } break;
            }
            btnServer.Content = serverStatus;
        }

        #endregion
    }
}
