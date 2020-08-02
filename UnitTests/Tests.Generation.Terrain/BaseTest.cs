using Generation.Terrain.Utils;
using Xunit.Abstractions;

namespace Tests.Generation.Terrain
{
    public abstract class BaseTest
    {
        protected static float[,] Heightmap = GetTestHeightMap();
        protected int Width;
        protected int Height;

        protected readonly ITestOutputHelper Output;

        public BaseTest(ITestOutputHelper output)
        {
            Width = Heightmap.GetLength(0);
            Height = Heightmap.GetLength(1);

            Output = output;
        }

        private static float[,] GetTestHeightMap()
        {
            var fullPath = "D:\\windows\\documents\\repositories\\procedural-terrain-generation\\UnitTests\\";
            var reader = new ReadWriteTerrain("test-heightmap", fullPath);
            
            return reader.ReadMatrix();
        }
    }
}