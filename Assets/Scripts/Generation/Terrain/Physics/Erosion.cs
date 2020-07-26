namespace Generation.Terrain.Physics.Erosion
{
    public interface Erosion
    {
        float[,] Erode(float[,] heightMap);
    }
}
