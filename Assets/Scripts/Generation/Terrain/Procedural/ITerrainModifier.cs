namespace Generation.Terrain.Procedural
{
    public interface ITerrainModifier
    {
        void Apply(float[,] heightmap);
    }
}