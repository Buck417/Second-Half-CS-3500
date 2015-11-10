using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Model
{
    /// <summary>
    /// Constructor for creating a cube, defining its position, unique ID, color, name,
    /// type and mass. Sets a width defined by its mass.
    /// </summary>
    public class Cube
    {
        public int UID;
        public double X, Y;
        public string Color, Name;
        public bool Food;
        public double Mass;
        public double Width;

        public Cube(int uID, double x, double y, string color, string name, bool food, double mass)
        {
            this.UID = uID;
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Name = name;
            this.Food = food;
            this.Mass = mass;
            this.Width = Math.Sqrt(mass);
        }


        public static Cube Create(string json)
        {
            return JsonConvert.DeserializeObject<Cube>(json);
        }
    }

    /// <summary>
    /// This is basically where all the cubes live, as well as the max dimensions for the world.
    /// </summary>
    public class World
    {
        int Max_Height, Max_Width;

        private Dictionary<int, Cube> cubes;

        /// <summary>
        /// This takes care of what we need to do with a cube, depending on whether it exists and if it has mass.
        /// </summary>
        /// <param name="cube"></param>
        public void ProcessIncomingCube(Cube cube)
        {
            bool cubeExists = cubes.ContainsKey(cube.UID);

            if (cubeExists)
            {
                //If mass is 0, that means the cube has been eaten/destroyed, and should no longer be shown.
                if (cube.Mass == 0)
                {
                    cubes.Remove(cube.UID);
                }

                //If the mass isn't 0, it means the cube exists, and should be updated. The location may change, size, etc.
                else
                {
                    cubes[cube.UID] = cube;
                }
            }

            //If the cube doesn't exist, only add it if it has a mass.
            else
            {
                if (cube.Mass > 0)
                {
                    cubes.Add(cube.UID, cube);
                }
            }

        }
    }
}
