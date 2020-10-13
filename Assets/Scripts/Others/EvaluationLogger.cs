using System.IO;
using System;

namespace TerrainGeneration.Analytics
{
    public static class EvaluationLogger
    {
        private static string _destination = Directory.GetCurrentDirectory() + "/logs-terrain/";
        public static string Destination
        { 
            get => _destination;
            set => _destination = $"{Directory.GetCurrentDirectory()}/logs-terrain/{value}/";
        }

        public static void RecordValue(string label, int terrainLength, string value)
        {
            System.IO.Directory.CreateDirectory(Destination);
            string filename = $"{label}_{terrainLength}x{terrainLength}";

            using (StreamWriter writer = File.AppendText($"{Destination}{filename}.log"))
            {
                writer.WriteLine(value);
            }
        }
    }
}
