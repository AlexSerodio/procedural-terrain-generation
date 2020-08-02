using System.IO;

namespace Generation.Terrain.Utils
{
    public class ReadWriteTerrain
    {
        private string filePathAndName;
        private const string extension = ".pgt";

        public ReadWriteTerrain(string filename, string path = ".\\Assets\\SavedTerrains\\")
        {
            filePathAndName = path + filename + extension;
        }

        public void WriteMatrix(float[,] matrix)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePathAndName, FileMode.Create)))
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

        public float[,] ReadMatrix()
        {
            float[,] matrix = null;

            if (File.Exists(filePathAndName))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filePathAndName, FileMode.Open)))
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
            }

            return matrix;
        }
    }
}