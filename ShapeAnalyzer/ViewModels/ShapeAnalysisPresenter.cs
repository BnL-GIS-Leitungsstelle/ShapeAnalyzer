using ShapeAnalyzer.Models;
using ShapeAnalyzer.Utils;
using ShapeAnalyzer.ViewModels.Mvvm;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ShapeAnalyzer.ViewModels
{
    public class ShapeAnalysisPresenter : Presenter
    {
        private readonly ShapeAnalysisModel _model;
        private string _shapePath;
        private bool _calculateKantoneIntersects;
        private bool _calculateGemeindeIntersects;
        private string _output;
        private string _status;
        private bool? _success;
        private bool _isCalculating;

        public ShapeAnalysisPresenter(ShapeAnalysisModel model)
        {
            _model = model;

            UpdateStatus("Ready");
            CalculateGemeindeIntersects = true;
            CalculateKantoneIntersects = true;
        }

        public string ShapePath
        {
            get => _shapePath;
            set => Update(ref _shapePath, value);
        }

        public bool CalculateKantoneIntersects
        {
            get => _calculateKantoneIntersects;
            set => Update(ref _calculateKantoneIntersects, value);
        }

        public bool CalculateGemeindeIntersects
        {
            get => _calculateGemeindeIntersects;
            set => Update(ref _calculateGemeindeIntersects, value);
        }

        public string Output
        {
            get => _output;
            set => Update(ref _output, value);
        }

        public string Status
        {
            get => _status;
            set => Update(ref _status, value);
        }

        public bool Success
        {
            get => _success ?? false;
            set => Update(ref _success, value);
        }

        public bool IsCalculating
        {
            get => _isCalculating;
            set => Update(ref _isCalculating, value);
        }

        public ICommand ShowShpFileDialogCommand => new Command(_ => { ShapePath = OpenFileDialogUtils.ShowOpenFileDialog("shp files (*.shp)|*.shp"); });

        public ICommand RunAnalysisCommand => new Command(async _ =>
        {
            IsCalculating = true;

            (Output, Success) = await Task.Run(() => 
            { 
                return _model.AnalyzeShapeFile(
                    ShapePath, CalculateKantoneIntersects, CalculateGemeindeIntersects, new Progress<string>(UpdateStatus)); 
            });

            UpdateStatus("Ready");
            IsCalculating = false;
        });

        private void UpdateStatus(string status)
        {
            Status = $"{status}";
        }
    }
}
