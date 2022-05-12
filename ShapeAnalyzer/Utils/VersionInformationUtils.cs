using System.IO;
using System.Reflection;

namespace ShapeAnalyzer.Utils
{
    public static class VersionInformationUtils
    {
        public static string GetVersion()
        {
            return $"v{Assembly.GetExecutingAssembly().GetName().Version}";
        }

        public static string GetBuildDate()
        {
            var buildDate = File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
            return $"{buildDate:dd.MM.yyyy}";
        }
    }
}
