using NLog;
using ShapeAnalyzer.Geo;
using System;
using System.IO;

namespace ShapeAnalyzer.Models
{
    public class ShapeAnalysisModel
    {
        private readonly ShapeFileAnalyzer _shapeFileAnalyzer;
        private readonly Logger _logger;

        public ShapeAnalysisModel()
        {
            _shapeFileAnalyzer = new ShapeFileAnalyzer();
            _logger = LogManager.GetCurrentClassLogger();
        }

        public (string, bool) AnalyzeShapeFile(
            string path, bool calculateKantoneIntersects, bool calculateGemeindeIntersects, IProgress<string> progress)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path) || !path.ToLower().EndsWith(".shp"))
            {
                return ("Please select a .shp file and try again.", false);                
            }
            
            _logger.Info("------ Start Analysis ------");
            return _shapeFileAnalyzer.RunAnalysis(
                path, calculateKantoneIntersects, calculateGemeindeIntersects, progress);           
        }
    }
}
