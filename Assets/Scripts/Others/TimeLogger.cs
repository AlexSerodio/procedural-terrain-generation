using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

namespace TerrainGeneration.Analytics
{
    public static class TimeLogger
    {
        private static List<float> _times;
        private static string _timestamp;
        private static string _name;
        private static Stopwatch _stopWatch = new Stopwatch();

        private static string DATE_FORMAT = "dd-MM-yyyy-HH-mm-ss";

        private static string Destination { get => Directory.GetCurrentDirectory() + "/terrain-logs"; }
        private static string FileName { get => $"{_name}_{_timestamp}.log"; }

        public static void Start(LoggerType type)
        {
            System.IO.Directory.CreateDirectory(Destination);

            _times = new List<float>();
            _name = GetAlgorithmName(type);
            _timestamp = DateTime.Now.ToString(DATE_FORMAT);

            _stopWatch.Start();
        }

        public static void RegisterTimeInMilliseconds()
        {
            _stopWatch.Stop();
            _times.Add(_stopWatch.Elapsed.Milliseconds);
            _stopWatch.Reset();
        }

        public static void SaveLog()
        {
            using (StreamWriter writer = File.AppendText($"{Destination}/{FileName}"))
            {
                writer.WriteLine("Algorithm:," + _name);
                writer.WriteLine("X Axis:, Iterations");
                writer.WriteLine("Y Axis:, Duration (ms)");

                _times.ForEach(time => writer.Write(time + ","));

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