using UnityEngine;

namespace Generation.Terrain.Procedural
{
    public class MidpointDisplacement : ITerrainModifier
    {
        public float HeightMin { get; set; }
        public float HeightMax { get; set; }
        public float HeightDampenerPower { get; set; }
        public float Roughness { get; set; }

        public MidpointDisplacement()
        {
            HeightMin = -2f;
            HeightMax = 2f;
            HeightDampenerPower = 2.0f;
            Roughness = 2.0f;
        }

        // TODO: refactor so that it is no longer necessary to use the UnityEngine namespace
        public void Apply(float[,] heightmap)
        {
            int width = heightmap.GetLength(0) - 1;
            int squareSize = width;
            float heightMin = HeightMin;
            float heightMax = HeightMax;
            // float roughness = 2.0f;
            float heightDampener = (float)Mathf.Pow(HeightDampenerPower, -1 * Roughness);

            int endX, endY;
            int midX, midY;
            int pMidXLeft, pMidXRight, pMidYUp, pMidYDown;

            while (squareSize > 0)
            {
                // diamond step
                for (int x = 0; x < width; x += squareSize)
                {
                    for (int y = 0; y < width; y += squareSize)
                    {
                        endX = (x + squareSize);
                        endY = (y + squareSize);

                        midX = (int)(x + squareSize / 2.0f);
                        midY = (int)(y + squareSize / 2.0f);

                        heightmap[midX, midY] = (float)((heightmap[x, y] +
                                                            heightmap[endX, y] +
                                                            heightmap[x, endY] +
                                                            heightmap[endX, endY]) / 4.0f +
                                                            UnityEngine.Random.Range(heightMin, heightMax));
                    }
                }

                // square step
                for (int x = 0; x < width; x += squareSize)
                {
                    for (int y = 0; y < width; y += squareSize)
                    {
                        endX = (x + squareSize);
                        endY = (y + squareSize);

                        midX = (int)(x + squareSize / 2.0f);
                        midY = (int)(y + squareSize / 2.0f);

                        pMidXRight = (int)(midX + squareSize);
                        pMidYUp = (int)(midY + squareSize);
                        pMidXLeft = (int)(midX - squareSize);
                        pMidYDown = (int)(midY - squareSize);

                        // if pmid values are off the limit, ignore them
                        if (pMidXLeft <= 0 || pMidYDown <= 0 || pMidXRight >= width - 1 || pMidYUp >= width - 1) 
                            continue;

                        //Calculate the square value for the bottom side  
                        heightmap[midX, y] = (float)((heightmap[midX, midY] +
                                                        heightmap[x, y] +
                                                        heightmap[midX, pMidYDown] +
                                                        heightmap[endX, y]) / 4.0f +
                                                        UnityEngine.Random.Range(heightMin, heightMax));

                        //Calculate the square value for the top side   
                        heightmap[midX, endY] = (float)((heightmap[x, endY] +
                                                        heightmap[midX, midY] +
                                                        heightmap[endX, endY] +
                                                        heightmap[midX, pMidYUp]) / 4.0f +
                                                        UnityEngine.Random.Range(heightMin, heightMax));

                        //Calculate the square value for the left side   
                        heightmap[x, midY] = (float)((heightmap[x, y] +
                                                        heightmap[pMidXLeft, midY] +
                                                        heightmap[x, endY] +
                                                        heightmap[midX, midY]) / 4.0f +
                                                        UnityEngine.Random.Range(heightMin, heightMax));
                        
                        //Calculate the square value for the right side   
                        heightmap[endX, midY] = (float)((heightmap[midX, y] +
                                                        heightmap[midX, midY] +
                                                        heightmap[endX, endY] +
                                                        heightmap[pMidXRight, midY]) / 4.0f +
                                                        UnityEngine.Random.Range(heightMin, heightMax));
                    }
                }

                // update step
                squareSize = (int)(squareSize / 2.0f);
                heightMin *= heightDampener;
                heightMax *= heightDampener;
            }
        }
    }
}