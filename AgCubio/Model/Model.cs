using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Cube
    {
        public int UID, X, Y;
        public string Color, Name;
        public bool Food;
        public double Mass;

        public Cube(int uID, int x, int y, string color, string name, bool food, double mass)
        {
            this.UID = uID;
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Name = name;
            this.Food = food;
            this.Mass = mass;
        }
    }

    /// <summary>
    /// This is basically where all the cubes live, as well as the max dimensions for the world.
    /// </summary>
    public class World
    {
        int Max_Height, Max_Width;
        Dictionary<int, Cube> Cubes;

        public World()
        {
            Cubes = new Dictionary<int, Cube>();
        }

        /// <summary>
        /// Helper method for adding a cube to display in the GUI
        /// </summary>
        /// <param name="cube"></param>
        public void AddCube(Cube cube)
        {
            Cubes.Add(cube.UID, cube);
        }
    }
}
