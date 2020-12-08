namespace Terrain
{
    public interface ITerrainModifier
    {
        void Apply(float[,] heightmap);
    }
}
