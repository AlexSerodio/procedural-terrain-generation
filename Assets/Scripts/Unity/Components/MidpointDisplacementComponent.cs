using System;
using System.Collections.Generic;
using Generation.Terrain.Procedural;
using UnityEngine;

namespace Unity.Components
{
    [ExecuteInEditMode]
    public class MidpointDisplacementComponent : BaseComponent
    {
        public float HeightDampenerPower = 2.0f;
        public float HeightMin = -2f;
        public float HeightMax = 2f;
        public float Roughness = 2.0f;

        private MidpointDisplacement MidpointDisplacement = new MidpointDisplacement();
        private const int chunksAmountPerRow = MeshGenerator.chunksAmountPerRow;

        public override void UpdateComponent()
        {
            float[,] heightmap = GetTerrainHeight();
        
            MidpointDisplacement.HeightDampenerPower = HeightDampenerPower;
            MidpointDisplacement.HeightMax = HeightMax;
            MidpointDisplacement.HeightMin = HeightMin;
            MidpointDisplacement.Roughness = Roughness;

            int roundedChunksSize = (heightmap.GetLength(0) + chunksAmountPerRow - 1) / chunksAmountPerRow;
            // Debug.Log($"Heightmap Dimensions: [{heightmap.GetLength(0)}, {heightmap.GetLength(1)}]");
            // Debug.Log($"Chunks Size: {roundedChunksSize}");


            if(chunksAmountPerRow > 1)
            {
                List<float[,]> chunks = SplitInChunks(heightmap, roundedChunksSize, roundedChunksSize);

                foreach (var chunk in chunks)
                    MidpointDisplacement.Apply(chunk);

                heightmap = CombineChunks(chunks, chunksAmountPerRow);
            }
            else
            {
                MidpointDisplacement.Apply(heightmap);
            }
            
            UpdateTerrainHeight(heightmap);
        }

        private List<float[,]> SplitInChunks(float[,] matrix, int row, int column)
        {
            // int chunkcount = (matrix.GetLength(0) * matrix.GetLength(1)) / (row * column);
            int chunkcount = chunksAmountPerRow * 2;
            // int roundedChunkscount = ((matrix.GetLength(0) * matrix.GetLength(1)) + (row * column) -1) / (row * column);

            // Debug.Log($"({matrix.GetLength(0)} * {matrix.GetLength(1)}) / ({row} * {column})");
            // Debug.Log(((matrix.GetLength(0) * matrix.GetLength(1)) + (row * column) -1) / (row * column));
            // Debug.Log(chunksAmountPerRow * chunksAmountPerRow);


            List<float[,]> chunkList = new List<float[,]>();
            float[,] chunk = new float[row, column];

            var byteLength = sizeof(float) * chunk.Length;
            for (int i = 0; i < chunkcount; i++)
            {
                chunk = new float[row, column];
                Buffer.BlockCopy(matrix, byteLength * i, chunk, 0, byteLength);

                chunkList.Add(chunk);
            }
            Debug.Log("Chunks Count: " + chunkList.Count);
            Debug.Log($"Chunks Dimensions: {chunkList[0].GetLength(0)}, {chunkList[0].GetLength(1)}");

            return chunkList;
        }

        private float[,] CombineChunks(List<float[,]> chunks, int chunksAmountPerRow)
        {
            int size = chunks[0].GetLength(0) * chunksAmountPerRow;

            float[,] matrix = new float[size, size];

            Debug.Log($"Matrix Size: [{matrix.GetLength(0)}, {matrix.GetLength(1)}]");

            int x = 0, y = 0;
            for (int c = 0; c < chunks.Count; c++, x++)
            {
                if(x >= chunksAmountPerRow)
                {
                    x = 0; y++;
                }

                int xOffset = (chunks[c].GetLength(0)) * x;
                int yOffset = (chunks[c].GetLength(1)) * y;

                for (int i = 0; i < chunks[c].GetLength(0); i++)
                {
                    for (int j = 0; j < chunks[c].GetLength(1); j++)
                    {
                        matrix[i+xOffset, j+yOffset] = chunks[c][i, j];
                    }
                }
            }

            return matrix;
        }
    }
}
