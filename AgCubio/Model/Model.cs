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
    /// type and mass. Sets a width defined by its mass. Adds function for converting JSON
    /// string into a cube.
    /// </summary>
    public class Cube
    {
        [JsonProperty]
        public int UID;
        [JsonProperty]
        public int X, Y, Color;
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public bool Food;
        [JsonProperty]
        public double Mass;
        public int Width;

        [JsonConstructor]
        public Cube(double loc_x, double loc_y, int argb_color, int uID, bool food, string name, double mass)
        {
            this.UID = uID;
            this.X = (int)loc_x;
            this.Y = (int)loc_y;
            this.Color = argb_color;
            this.Name = name;
            this.Food = food;
            this.Mass = mass;
            this.Width = (int)(Math.Sqrt(mass));
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
        public int Scale = 1;
        public string Player_Name;
        public int Player_UID;
        public double Player_Start_Mass;
        public int xoff, yoff;

        private Dictionary<int, Cube> cubes = new Dictionary<int, Cube>();

        public Cube GetCube(int UID)
        {
            if (cubes.ContainsKey(UID))
                return cubes[UID];
            else return null;
        }

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
