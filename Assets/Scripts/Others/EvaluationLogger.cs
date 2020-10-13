using System.IO;
using System;

namespace TerrainGeneration.Analytics
{
    public static class EvaluationLogger
    {
        private static string filename;
        private const string DATE_FORMAT = "dd-MM-yyyy-HH-mm-ss";

        private static string _destination = Directory.GetCurrentDirectory() + "/logs-terrain/";
        public static string Destination
        { 
            get => _destination;
            set => _destination = $"{Directory.GetCurrentDirectory()}/logs-terrain/{value}/";
        }
        private static string Timestamp { get => DateTime.Now.ToString(DATE_FORMAT); }

        public static void Start(string label, int terrainLength)
        {
            System.IO.Directory.CreateDirectory(Destination);
            filename = $"{label}_{terrainLength}x{terrainLength}";
        }


        public static void RecordSingleTimeInMilliseconds(string label, int terrainLength, string value)
        {
            Start(label, terrainLength);

            using (StreamWriter writer = File.AppendText($"{Destination}{filename}.log"))
            {
                writer.WriteLine(value);
            }
        }
    }
}
