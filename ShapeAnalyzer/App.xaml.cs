using ShapeAnalyzer.Models;
using ShapeAnalyzer.Utils;
using ShapeAnalyzer.ViewModels;
using System.Windows;

namespace ShapeAnalyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);           

            var model = new ShapeAnalysisModel();
            var presenter = new ShapeAnalysisPresenter(model);
            var mainWindow = new MainWindow { DataContext = presenter };

            LoggingUtils.Init();

            mainWindow.Show();
        }
    }
}
