﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

namespace TerrainGeneration.Analytics
{
    public static class TimeLogger
    {
        private static List<int> records;
        private static string filename;
        private static Stopwatch stopWatch = new Stopwatch();
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

            records = new List<int>();
            filename = $"{label}_{terrainLength}x{terrainLength}";

            stopWatch.Start();
        }

        public static void RecordTimeInMilliseconds()
        {
            stopWatch.Stop();
            records.Add((int)stopWatch.Elapsed.TotalMilliseconds);
            stopWatch.Reset();
        }

        public static void RecordSingleTimeInMilliseconds()
        {
            stopWatch.Stop();
            records.Add((int)stopWatch.Elapsed.TotalMilliseconds);
            stopWatch.Reset();

            using (StreamWriter writer = File.AppendText($"{Destination}{filename}.log"))
            {
                records.ForEach(time => writer.Write(time + ", "));
            }
        }

        public static void SaveLog()
        {
            using (StreamWriter writer = File.AppendText($"{Destination}{filename}_{Timestamp}.log"))
            {
                writer.WriteLine("Algorithm:," + filename);
                writer.WriteLine("X Axis:, Iterations");
                writer.WriteLine("Y Axis:, Duration (ms)");

                records.ForEach(time => writer.Write(time + ","));

                writer.WriteLine();
            }
        }
    }
}
