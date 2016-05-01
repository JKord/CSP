#region namespace
using System;
using System.Windows;
using System.Windows.Controls;
using CSP.Core;
using CSP.Core.Util;
using CSP.Core.Model.Part;
#endregion

namespace CSP.Client
{
    public partial class ClientWindow : Window
    {
        #region Fields

        ChordTopoMap chordTopoMap = new ChordTopoMap();
        private ClientStatus clientStatus = ClientStatus.Close;

        #endregion

        public ClientWindow()
        {
            InitializeComponent();
            Logger.Show = s => tbConsole.Text += s + "\r\n";
        }

        #region Events Button Click

        private void BtnClient_Click(object sender, RoutedEventArgs e)
        {
            int port = 1006;
            try {
                port = int.Parse(tbPort.Text);
            } catch (Exception ignore) { }

            switch (clientStatus)
            {
                case ClientStatus.Open:
                {
                    clientStatus = ClientStatus.Close;
                    chordTopoMap.Close();
                } break;
                case ClientStatus.Close:
                {
                    clientStatus = ClientStatus.Open;
                    chordTopoMap.Connect(tbHost.Text, port);
                } break;
            }
            ((Button) sender).Content = clientStatus;

        }

        private void BtnSaveMapCSV_Click(object sender, RoutedEventArgs e) => chordTopoMap.GenerateCsv();
        private void BtnSaveMapJPG_Click(object sender, RoutedEventArgs e) => chordTopoMap.GenerateImage();

        private void BtnTest1_Click(object sender, RoutedEventArgs e) => JSRun.Execute("prog/test1.js");

        private void BtnSort_Click(object sender, RoutedEventArgs e) => chordTopoMap.RunSortTask();

        #endregion
    }
}
