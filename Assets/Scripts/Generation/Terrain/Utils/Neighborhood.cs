using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Generation.Terrain.Utils
{
    public static class Neighborhood
    {
        /// <summary>
        /// Retrieves all eight neighbors from the position informed, following Moore's neighborhood pattern.
        /// If the informed position is at the xMax or yMax limit, some neighbors may not exist and therefore 
        /// will not be returned.
        /// </summary>
        /// <param name="seed">The central position from which wants to recover the neighbors.</param>
        /// <param name="xMax">The maximum X position.</param>
        /// <param name="yMax">The maximum Y position.</param>
        /// <returns>A list with the neighbors found.</returns>
        public static List<Vector2> Moore(Vector2 seed, int xMax, int yMax)
        {
            HashSet<Vector2> neighbors = new HashSet<Vector2>();
            Vector2 min = Vector2.Zero;
            Vector2 max = new Vector2(xMax-1, yMax-1);
            
            neighbors.Add(Vector2.Clamp(new Vector2(seed.X-1, seed.Y+1), min, max));
            neighbors.Add(Vector2.Clamp(new Vector2(seed.X, seed.Y+1), min, max));
            neighbors.Add(Vector2.Clamp(new Vector2(seed.X+1, seed.Y+1), min, max));

            neighbors.Add(Vector2.Clamp(new Vector2(seed.X-1, seed.Y), min, max));
            neighbors.Add(Vector2.Clamp(new Vector2(seed.X+1, seed.Y), min, max));
            
            neighbors.Add(Vector2.Clamp(new Vector2(seed.X-1, seed.Y-1), min, max));
            neighbors.Add(Vector2.Clamp(new Vector2(seed.X, seed.Y-1), min, max));
            neighbors.Add(Vector2.Clamp(new Vector2(seed.X+1, seed.Y-1), min, max));
            
            // Ensures that the seed itself is not included in the neighbors.
            neighbors.Remove(seed);

            return neighbors.ToList();
        }

        /// <summary>
        /// Retrieves all four neighbors from the position informed, following Von Neumann's neighborhood pattern.
        /// If the informed position is at the xMax or yMax limit, some neighbors may not exist and therefore
        /// will not be returned.
        /// </summary>
        /// <param name="seed">The central position from which wants to recover the neighbors.</param>
        /// <param name="xMax">The maximum X position.</param>
        /// <param name="yMax">The maximum Y position.</param>
        /// <returns>A list with the neighbors found.</returns>
        public static List<Vector2> VonNeumann(Vector2 seed, int width, int height)
        {
            HashSet<Vector2> neighbors = new HashSet<Vector2>();
            Vector2 min = Vector2.Zero;
            Vector2 max = new Vector2(width-1, height-1);
            
            neighbors.Add(Vector2.Clamp(new Vector2(seed.X, seed.Y+1), min, max));

            neighbors.Add(Vector2.Clamp(new Vector2(seed.X-1, seed.Y), min, max));
            neighbors.Add(Vector2.Clamp(new Vector2(seed.X+1, seed.Y), min, max));
            
            neighbors.Add(Vector2.Clamp(new Vector2(seed.X, seed.Y-1), min, max));
            
            // Ensures that the seed itself is not included in the neighbors.
            neighbors.Remove(seed);

            return neighbors.ToList();
        }
    }
}