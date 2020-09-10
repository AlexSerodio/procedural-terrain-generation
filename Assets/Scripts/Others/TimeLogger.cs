using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

namespace TerrainGeneration.Analytics
{
    public static class TimeLogger
    {
        private static List<float> times;
        private static string timestamp;
        private static string name;
        private static Stopwatch stopWatch = new Stopwatch();
        private static string DATE_FORMAT = "dd-MM-yyyy-HH-mm-ss";

        private static string Destination { get => Directory.GetCurrentDirectory() + "/logs-terrain"; }
        private static string FileName { get => $"{name}_{timestamp}.log"; }

        public static void Start(LoggerType type, int terrainLength)
        {
            System.IO.Directory.CreateDirectory(Destination);

            times = new List<float>();
            name = $"{GetAlgorithmName(type)}_{terrainLength}x{terrainLength}";
            timestamp = DateTime.Now.ToString(DATE_FORMAT);

            stopWatch.Start();
        }

        public static void RecordTimeInMilliseconds()
        {
            stopWatch.Stop();
            times.Add(stopWatch.Elapsed.Milliseconds);
            stopWatch.Reset();
        }

        public static void RecordSingleTimeInMilliseconds()
        {
            stopWatch.Stop();
            times.Add(stopWatch.Elapsed.Milliseconds);
            stopWatch.Reset();

            using (StreamWriter writer = File.AppendText($"{Destination}/{name}.log"))
            {
                times.ForEach(time => writer.Write(time + ","));
            }
        }

        public static void SaveLog()
        {
            using (StreamWriter writer = File.AppendText($"{Destination}/{FileName}"))
            {
                writer.WriteLine("Algorithm:," + name);
                writer.WriteLine("X Axis:, Iterations");
                writer.WriteLine("Y Axis:, Duration (ms)");

                times.ForEach(time => writer.Write(time + ","));

                writer.WriteLine();
            }
        }

        private static string GetAlgorithmName(LoggerType type)
        {
            switch (type)
            {
                case LoggerType.CPU_DIAMOND_SQUARE:
                    return "cpu-diamond-square";
                case LoggerType.CPU_THERMAL_EROSION:
                    return "cpu-thermal-erosion";
                case LoggerType.GPU_DIAMOND_SQUARE:
                    return "gpu-diamond-square";
                case LoggerType.GPU_THERMAL_EROSION:
                    return "gpu-thermal-erosion";
                default:
                    return "unknown";
            }
        }
    }
}
