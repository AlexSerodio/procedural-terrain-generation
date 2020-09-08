using Generation.Terrain.Procedural.GPU;
using Generation.Terrain.Procedural;
using System.Diagnostics;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class DiamondSquareComponent : BaseComponent
    {
        public int resolution;
        public float height;
        public ComputeShader shader;
        public bool useGPU;

        private DiamondSquare diamondSquare = new DiamondSquare();
        private DiamondSquareGPU diamondSquareGPU = new DiamondSquareGPU();

        public override void UpdateComponent()
        {
            float[,] heightmap = base.GetTerrainHeight();

            if(!useGPU) {
                diamondSquare.Resolution = base.meshGenerator.resolution;
                diamondSquare.Height = height;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                diamondSquare.Apply(heightmap);

                stopWatch.Stop ();
                UnityEngine.Debug.Log($"DiamondSquare on CPU with {diamondSquare.Resolution}x{diamondSquare.Resolution} vertices took {stopWatch.Elapsed.Milliseconds}ms.");
                stopWatch.Reset ();
            }
            else
            {
                diamondSquareGPU.Resolution = base.meshGenerator.resolution;
                diamondSquareGPU.Height = height;
                diamondSquareGPU.Shader = shader;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                diamondSquareGPU.Apply(heightmap);
            
                stopWatch.Stop ();
                UnityEngine.Debug.Log($"DiamondSquare on GPU with {diamondSquareGPU.Resolution}x{diamondSquareGPU.Resolution} vertices took {stopWatch.Elapsed.Milliseconds}ms.");
                stopWatch.Reset ();
            }
            
            base.UpdateTerrainHeight(heightmap);
        }
    }
}
