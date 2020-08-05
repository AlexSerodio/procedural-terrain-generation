using Generation.Terrain.Utils;
using Xunit.Abstractions;

namespace Tests.Generation.Terrain
{
    public abstract class BaseTest
    {
        // protected static float[,] Heightmap = ReadHeightMap("heightmap-1");
        // protected int Width;
        // protected int Height;

        protected readonly ITestOutputHelper Output;

        public BaseTest(ITestOutputHelper output)
        {
            // Width = Heightmap.GetLength(0);
            // Height = Heightmap.GetLength(1);

            Output = output;
        }

        protected static float[,] ReadHeightMap(string filename)
        {
            var fullPath = "D:\\windows\\documents\\repositories\\procedural-terrain-generation\\Heighmaps\\";
            var reader = new ReadWriteTerrain(filename, fullPath);
            
            return reader.ReadMatrix();
        }
    }
}