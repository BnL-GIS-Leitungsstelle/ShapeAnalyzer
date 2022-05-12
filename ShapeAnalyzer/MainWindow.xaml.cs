using ShapeAnalyzer.Models;
using ShapeAnalyzer.Utils;
using ShapeAnalyzer.ViewModels;
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

namespace ShapeAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            statusBuildDate.Content = VersionInformationUtils.GetBuildDate();
            statusVersion.Content = VersionInformationUtils.GetVersion();            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ((ShapeAnalysisPresenter)this.DataContext).PropertyChanged += MainWindow_PropertyChanged_UpdateStatusColor;
        }

        private void MainWindow_PropertyChanged_UpdateStatusColor(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ShapeAnalysisPresenter.Success))
            {
                if (((ShapeAnalysisPresenter)sender).Success)
                {
                    buttonAnalysis.Background = Brushes.LightGreen;
                }
                else
                {
                    buttonAnalysis.Background = Brushes.PaleVioletRed;
                }
            }
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            new HelpWindow().ShowDialog();
        }
    }
}
