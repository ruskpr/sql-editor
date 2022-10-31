using ProductEditor;
using System;
using System.Diagnostics;
using System.Windows;


namespace SQLEditor
{
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
            this.Closing += SplashWindow_Closing;
            btnClose.Click += BtnClose_Click;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => this.Close();

        private void SplashWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var destinationurl = "https://github.com/ruskpr/SQL-Editor";
            var sInfo = new System.Diagnostics.ProcessStartInfo(destinationurl)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
        }
    }
}
