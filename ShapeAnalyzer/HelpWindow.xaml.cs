using ShapeAnalyzer.Utils;
using System;
using System.IO;
using System.Windows;

namespace ShapeAnalyzer
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();

            pdfWebBrowser.Navigate(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Docs\Bedienungsanleitung_Shape_Analyzer.pdf"));
            FillInformationBox();
        }

        private void FillInformationBox()
        {
            labelInformation.Content = $"{nameof(ShapeAnalyzer)}{Environment.NewLine}" +
                $"{VersionInformationUtils.GetVersion()}{Environment.NewLine}" +
                $"{VersionInformationUtils.GetBuildDate()}";
        }

        private void SupportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MailUtils.CreateAndOpenMailMessage(
                    "Helmut.Recher@bafu.admin.ch", "[ShapeAnalyzer] Support Request", LoggingUtils.GetLogFilePath());
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Error opening default Mail program. Please send Mail manually.", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }
    }
}
