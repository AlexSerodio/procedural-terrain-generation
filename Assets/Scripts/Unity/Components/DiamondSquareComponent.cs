using Generation.Terrain.Procedural.GPU;
using Generation.Terrain.Procedural;
using Generation.Terrain.Evaluation;
using TerrainGeneration.Analytics;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class DiamondSquareComponent : BaseComponent
    {
        public string seed;
        public ComputeShader shader;
        public bool useGPU;

        private DiamondSquare diamondSquare;

        public override void UpdateComponent()
        {
            float[,] heightmap = base.GetTerrainHeight();
            int resolution = base.meshGenerator.resolution;
            int intSeed = CovertStringSeedToInt(seed);

            if (useGPU)
                diamondSquare = new DiamondSquareGPU(resolution, shader, intSeed);
            else
                diamondSquare = new DiamondSquare(resolution, intSeed);

            TimeLogger.Start(diamondSquare.GetType().Name, diamondSquare.Resolution);
            diamondSquare.Apply(heightmap);
            TimeLogger.RecordSingleTimeInMilliseconds();

            Debug.Log($"Erosion Score Diamond-Square: {ErosionScore.Evaluate(heightmap)}");
            Debug.Log($"Benford's Law Diamond-Square: {BenfordsLaw.Evaluate(heightmap)}");
        
            base.UpdateTerrainHeight(heightmap);
        }

        private int CovertStringSeedToInt(string stringSeed)
        {
            if(string.IsNullOrWhiteSpace(stringSeed))
                return new System.Random().Next();

            var md5Hasher = System.Security.Cryptography.MD5.Create();
            var hashed = md5Hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(stringSeed));
            return System.BitConverter.ToInt32(hashed, 0);
        }
    }
}
