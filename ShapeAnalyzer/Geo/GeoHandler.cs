using System;
using System.Collections.Generic;
using System.Linq;
using OGCToolsNetCoreLib.DataAccess;
using OGCToolsNetCoreLib.Models;
using OSGeo.OSR;

namespace ShapeAnalyzer.Geo
{
    /// <summary>
    /// wraps calls to OgcTools-Library
    /// </summary>
    public class GeoHandler
    {
        private readonly GeoDataSourceAccessor _datasourceAccessor;

        public GeoHandler()
        {
            _datasourceAccessor = new GeoDataSourceAccessor();
        }


        public double GetAreaInHa(IOgctLayer layer)
        {
            return layer.CalculateArea() / 10000;
        }

        public double ConvertAreaToHa(double area, double linearUnits, int decimalPlaces = 4)
        {
            if (area > 0)
            {
                area = (area * linearUnits) / 10000;
            }

            return Math.Round(area, decimalPlaces);
        }


        public OverlapResult CalculateOverlap(IOgctLayer referenceLayer, IOgctLayer overlapLayer)
        {
            var intersectionArea = CalculateIntersectionArea(referenceLayer, overlapLayer);

            // Make sure Overlap is not bigger than area of checked Layer
            overlapLayer.ResetReading();
            var overlapLayerArea = overlapLayer.CalculateArea();
            if (intersectionArea > overlapLayerArea)
            {
                intersectionArea = overlapLayerArea;
            }

            var spatialRef = referenceLayer.GetLinearUnits();
            return new OverlapResult
            {
                OverlapAreaInHa = ConvertAreaToHa(intersectionArea, spatialRef),
                OverlapPercentage = Math.Round((intersectionArea / overlapLayerArea) * 100, 4),
                NonOverlappingAreaInHa = ConvertAreaToHa(overlapLayerArea - intersectionArea, spatialRef)
            };
        }

        public double CalculateIntersectionArea(IOgctLayer referenceLayer, IOgctLayer overlapLayer)
        {
            var overlapLayerFeatureIds = GetFeatureIds(overlapLayer);

            double intersectionArea = 0;

            foreach (var featureIdRefLayer in GetFeatureIds(referenceLayer))
            {
                using var refFeature = referenceLayer.OpenFeatureByFid(featureIdRefLayer);
                using var refGeometry = refFeature.OpenGeometry();
                var refProjection = refGeometry.GetWkt();

                foreach (var featureIdOverlapLayer in overlapLayerFeatureIds)
                {
                    using var overlapFeature = overlapLayer.OpenFeatureByFid(featureIdOverlapLayer);
                    using var overlapGeometry = overlapFeature.OpenGeometry();

                    var overlapProjection =overlapGeometry.GetWkt();
                    if (refProjection != overlapProjection)
                    {
                        Reproject(refGeometry, overlapGeometry);
                    }

                    using var intersect = refGeometry.GetAndOpenIntersection(overlapGeometry);
                    intersectionArea += intersect.Area;
                }
            }

            return intersectionArea;
        }

        public IEnumerable<string> GetDistinctIntersectFieldValues(IOgctLayer referenceLayer, string referenceLayerFieldToReturn, IOgctLayer overlapLayer)
        {
            var overlapLayerFeatureIds = GetFeatureIds(overlapLayer);

            HashSet<string> intersectFieldValues = new HashSet<string>();

            foreach (var featureIdRefLayer in GetFeatureIds(referenceLayer))
            {
                using var refFeature = referenceLayer.OpenFeatureByFid(featureIdRefLayer);
                using var refGeometry = refFeature.OpenGeometry();
                var refProjection = refGeometry.GetWkt();

                foreach (var featureIdOverlapLayer in overlapLayerFeatureIds)
                {
                    using var overlapFeature = overlapLayer.OpenFeatureByFid(featureIdOverlapLayer);
                    using var overlapGeometry = overlapFeature.OpenGeometry();

                    var overlapProjection = overlapGeometry.GetWkt();
                    if (refProjection != overlapProjection)
                    {
                        Reproject(refGeometry, overlapGeometry);
                    }

                    if (overlapGeometry.Intersects(refGeometry))
                    {
                        intersectFieldValues.Add(refFeature.GetFieldAsString(referenceLayerFieldToReturn));
                    }
                }
            }

            return intersectFieldValues.ToArray();
        }

        public int Reproject(IOgctGeometry referenceGeometry, IOgctGeometry geometryToTransform)
        {
            return geometryToTransform.Reproject(referenceGeometry);
        }

        public IEnumerable<long> GetFeatureIds(IOgctLayer layer)
        {
            layer.ResetReading();

            IOgctFeature feature;
            while ((feature = layer.OpenNextFeature()) != null)
            {
                var featureId = feature.FID;
                feature.Dispose();
                yield return featureId;
            }
        }
    }
}
