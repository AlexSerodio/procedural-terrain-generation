using System;
using System.IO;

namespace Terrain.Utils
{
    /// <summary>
    /// Implements functions to serialize and deserialize heightmaps to binary files.
    /// All the files are serialized with the '.pgt' extension and therefore all 
    /// deserialized files must have the same extension.
    /// </summary>
    public static class HeightmapSerializer
    {
        private const string extension = ".pgt";

        /// <summary>
        /// Serialize the heightmap matrix argument to the filename location.
        /// </summary>
        /// <param name="matrix">The heightmap to serialize.</param>
        /// <param name="filename">The file path and name to which serialize.</param>
        public static void Serialize(float[,] matrix, string filename)
        {
            if (matrix == null)
                throw new ArgumentNullException("The matrix argument cannot be null.");

            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("The filename argument cannot be null or empty.");

            if (!filename.EndsWith(extension))
                filename += extension;

            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
                int width = matrix.GetLength(0);
                int height = matrix.GetLength(1);

                writer.Write(width);
                writer.Write(height);

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                        writer.Write(matrix[i, j]);
                }
            }
        }

        /// <summary>
        /// Deserialize the heightmap file to a matrix.
        /// The filename must end with the '.pgt' extension.
        /// </summary>
        /// <param name="filename">The file path and name null which the heightmap is serialized.</param>
        /// <returns>The deserialized heightmap matrix.</returns>
        public static float[,] Deserialize(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("The filename argument cannot be null or empty.");

            if (!filename.EndsWith(extension))
                filename += extension;

            if (!File.Exists(filename))
                throw new FileNotFoundException($"Couldn't find the file {filename}. Make sure the file exists and has the '{extension}' extension.");

            float[,] matrix = null;
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                matrix = new float[width, height];

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                        matrix[i, j] = reader.ReadSingle();
                }
            }

            return matrix;
        }
    }
}
