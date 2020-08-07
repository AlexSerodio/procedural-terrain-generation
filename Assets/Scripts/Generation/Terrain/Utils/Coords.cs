namespace Generation.Terrain.Utils
{
    /// <summary>
    /// Represents an X, Y coordinate on the plane.
    /// </summary>
    public struct Coords
    {
        public int X;
        public int Y;

        public Coords(int x, int y) {
            X = x;
            Y = y;
        }
    }
}