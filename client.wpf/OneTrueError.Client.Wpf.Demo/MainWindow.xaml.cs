using System;
using System.Windows;

namespace OneTrueError.Client.Wpf.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var url = new Uri("http://localhost/onetrueerror/");
            OneTrue.Configuration.Credentials(url, "yourAppKey", "yourSharedSecret");
            OneTrue.Configuration.CatchWpfExceptions();
            OneTrue.Configuration.UserInteraction.AskUserForDetails = true;
            OneTrue.Configuration.UserInteraction.AskUserForPermission = true;
            OneTrue.Configuration.UserInteraction.AskForEmailAddress = true;
//            Application.Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;
            InitializeComponent();
        }

        private void ThrowException(object sender, RoutedEventArgs e)
        {
            throw new Exception("Demo exception");
        }
    }
}
