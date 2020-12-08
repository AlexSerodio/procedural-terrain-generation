using System.Collections.Generic;

namespace Terrain.Utils
{
    public static class Neighborhood
    {
        /// <summary>
        /// Retrieves all eight neighbors from the position informed, following Moore's neighborhood pattern.
        /// If the informed position is at the xMax or yMax limit, some neighbors may not exist and therefore 
        /// will not be returned.
        /// </summary>
        /// <param name="seed">The central position from which wants to recover the neighbors.</param>
        /// <param name="width">The maximum X position.</param>
        /// <param name="height">The maximum Y position.</param>
        /// <returns>A list with the neighbors found.</returns>
        public static List<Coords> Moore(Coords seed, int width, int height)
        {
            List<Coords> neighbors = new List<Coords>();

            if (seed.X > 0)
                neighbors.Add(new Coords(seed.X - 1, seed.Y));

            if (seed.Y > 0)
                neighbors.Add(new Coords(seed.X, seed.Y - 1));

            if (seed.X < width - 1)
                neighbors.Add(new Coords(seed.X + 1, seed.Y));

            if (seed.Y < height - 1)
                neighbors.Add(new Coords(seed.X, seed.Y + 1));

            if (seed.X > 0 && seed.Y > 0)
                neighbors.Add(new Coords(seed.X - 1, seed.Y - 1));

            if (seed.X > 0 && seed.Y < height - 1)
                neighbors.Add(new Coords(seed.X - 1, seed.Y + 1));

            if (seed.X < width - 1 && seed.Y < height - 1)
                neighbors.Add(new Coords(seed.X + 1, seed.Y + 1));

            if (seed.X < width - 1 && seed.Y > 0)
                neighbors.Add(new Coords(seed.X + 1, seed.Y - 1));

            return neighbors;
        }

        /// <summary>
        /// Retrieves all four neighbors from the position informed, following Von Neumann's neighborhood pattern.
        /// If the informed position is at the xMax or yMax limit, some neighbors may not exist and therefore
        /// will not be returned.
        /// </summary>
        /// <param name="seed">The central position from which wants to recover the neighbors.</param>
        /// <param name="width">The maximum X position.</param>
        /// <param name="height">The maximum Y position.</param>
        /// <returns>A list with the neighbors found.</returns>
        public static List<Coords> VonNeumann(Coords seed, int width, int height)
        {
            List<Coords> neighbors = new List<Coords>();

            if (seed.X > 0)
                neighbors.Add(new Coords(seed.X - 1, seed.Y));

            if (seed.Y > 0)
                neighbors.Add(new Coords(seed.X, seed.Y - 1));

            if (seed.X < width - 1)
                neighbors.Add(new Coords(seed.X + 1, seed.Y));

            if (seed.Y < height - 1)
                neighbors.Add(new Coords(seed.X, seed.Y + 1));

            return neighbors;
        }
    }
}
