using NLog;
using OSGeo.OGR;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using OGCToolsNetCoreLib.DataAccess;
using OGCToolsNetCoreLib.Layer;
using OGCToolsNetCoreLib.Models;

namespace ShapeAnalyzer.Geo
{
    public class ShapeFileAnalyzer
    {
        private const string _kantonNameField = "NAME";
        private const string _gemeindeNameField = "NAME";

        private static readonly string _switzerlandShapesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\Switzerland_Shapes");
        private static readonly string _fullChShpPath = Path.Combine(_switzerlandShapesPath, "swissBOUNDARIES3D_1_3_TLM_LANDESGEBIET.shp");
        private static readonly string _kantoneShpPath = Path.Combine(_switzerlandShapesPath, "swissBOUNDARIES3D_1_3_TLM_KANTONSGEBIET.shp");
        private static readonly string _gemeindeShpPath = Path.Combine(_switzerlandShapesPath, "swissBOUNDARIES3D_1_3_TLM_HOHEITSGEBIET.shp");

        private readonly GeoHandler _geoHandler;
        private readonly Logger _logger;
        private readonly GeoDataSourceAccessor _geodataAccessor;

        public ShapeFileAnalyzer()
        {
            _geoHandler = new GeoHandler();
            _geodataAccessor = new GeoDataSourceAccessor();
            _logger = LogManager.GetCurrentClassLogger();

            if (!Directory.Exists(_switzerlandShapesPath))
            {
                ExtractArchive($"{_switzerlandShapesPath}.RequiredData", _switzerlandShapesPath);
            }
        }

        public (string, bool) RunAnalysis(
            string path, bool calculateKantoneIntersects, bool calculateGemeindeIntersects, IProgress<string> progress)
        {
            StringBuilder analysis = new StringBuilder();
            var success = false;

            try
            {
                _logger.Info($"Opening Shp: {path}");
                progress?.Report("Status: Opening Shp File...");
                using var ds = _geodataAccessor.OpenDatasource(path);
                using var layer = ds.OpenLayer(ds.GetLayerNames().First());

                progress?.Report("Status: Get Spatial Reference...");
                GetSpatialReference(layer, analysis);

                progress?.Report("Status: Calculate Area...");
                GetArea(layer, analysis);

                progress?.Report("Status: Compare To CH Area...");
                GetChAreaComparison(layer, analysis);

                if (calculateKantoneIntersects)
                {
                    progress?.Report("Status: Calculate Kantone Intersections...");
                    GetKantoneIntersections(layer, analysis);
                }

                if (calculateGemeindeIntersects)
                {
                    progress?.Report("Status: Calculate Gemeinde Intersections...");
                    GetGemeindeIntersections(layer, analysis);
                }

                success = true;
            }
            catch (Exception ex)
            {
                var message = $"Error running Analysis: {ex.Message}";
                _logger.Error(ex, message);
                analysis.Append(message);
            }

            return (analysis.ToString(), success);
        }

        private void GetSpatialReference(IOgctLayer layer, StringBuilder analysis)
        {
            _logger.Info("Get spatial reference");
            var layerDetails = layer.LayerDetails;
            analysis.Append($"---- Spatial Info ----{Environment.NewLine}" +
            $"Projection: {layerDetails.Projection.Name}");


            analysis.AppendLine();
            analysis.AppendLine();
        }

        private void GetArea(IOgctLayer layer, StringBuilder analysis)
        {
            _logger.Info("Calculate area");
            var area = _geoHandler.GetAreaInHa(layer);
            analysis.Append($"---- Area ----{Environment.NewLine}{area} ha");

            analysis.AppendLine();
            analysis.AppendLine();
        }

        private void GetChAreaComparison(IOgctLayer layer, StringBuilder analysis)
        {
            _logger.Info("Compare area to CH");
            using var ds = _geodataAccessor.OpenDatasource(_fullChShpPath);
            using var layerCh = ds.OpenLayer(ds.GetLayerNames().First());
            var overlapResult = _geoHandler.CalculateOverlap(layerCh, layer);

            if (overlapResult.OverlapPercentage < 100)
            {
                analysis.Append($"---- Area Outside CH ----{Environment.NewLine}" +
                    $"{overlapResult.NonOverlappingAreaInHa} ha   {100 - overlapResult.OverlapPercentage} %");

                analysis.AppendLine();
                analysis.AppendLine();
            }
        }

        private void GetKantoneIntersections(IOgctLayer layer, StringBuilder analysis)
        {
            _logger.Info("Get Intersections with Kantone.");
            analysis.Append("---- Kantone Intersects ----");
            GetIntersects(layer, _kantoneShpPath, _kantonNameField, analysis);
        }

        private void GetGemeindeIntersections(IOgctLayer layer, StringBuilder analysis)
        {
            _logger.Info("Get Intersections with Gemeinden.");
            analysis.Append("---- Gemeinde Intersects ----");
            GetIntersects(layer, _gemeindeShpPath, _gemeindeNameField, analysis);
        }

        private void GetIntersects(IOgctLayer layer, string referenceLayerPath, string outputField, StringBuilder analysis)
        {
            using var ds = _geodataAccessor.OpenDatasource(referenceLayerPath);
            using var referenceLayer = ds.OpenLayer(ds.GetLayerNames().First());
            var intersectedKantone = _geoHandler.GetDistinctIntersectFieldValues(referenceLayer, outputField, layer);

            analysis.Append($"{Environment.NewLine}{string.Join(Environment.NewLine, intersectedKantone)}");
            analysis.AppendLine();
            analysis.AppendLine();
        }

        private void ExtractArchive(string archivePath, string outputDir)
        {
            using (ZipArchive archive = ZipFile.OpenRead(archivePath))
            {
                _logger.Info($"Extract {archivePath} to folder {outputDir}.");
                archive.ExtractToDirectory(outputDir);
            }
        }
    }
}
