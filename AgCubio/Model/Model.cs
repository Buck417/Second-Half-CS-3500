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
        public int team_id;
        [JsonProperty]
        public int X, Y, Color;
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public bool Food;
        [JsonProperty]
        public int Mass;
        public int Width;
        
        [JsonConstructor]
        public Cube(double loc_x, double loc_y, int argb_color, int uID, int team_id, bool food, string name, double mass)
        {
            this.UID = uID;
            this.team_id = team_id;
            this.X = (int)loc_x;
            this.Y = (int)loc_y;
            this.Color = argb_color;
            this.Name = name;
            this.Food = food;
            this.Mass = (int)mass;
            this.Width = (int)(Math.Pow(mass, .53));
        }

        public static Cube Create(string json)
        {
            try {
                return JsonConvert.DeserializeObject<Cube>(json);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public int GetCenterX()
        {
            return X + (Width / 2);
        }

        public int GetCenterY()
        {
            return Y + (Width / 2);
        }
    }

    /// <summary>
    /// This is basically where all the cubes live, as well as the max dimensions for the world.
    /// </summary>
    public class World
    {
        public int Scale = 1;
        public string Player_Name;
        public int Player_UID;
        public double Player_Start_Mass;
        public int xoff, yoff;

        public Dictionary<int, Cube> cubes = new Dictionary<int, Cube>();

        public void AddPlayerCube(string json)
        {
            Cube cube = Cube.Create(json);
            ProcessIncomingCube(cube);
            Player_Start_Mass = cube.Mass;
            Player_Name = cube.Name;
            Player_UID = cube.UID;
        }

        public Cube GetPlayerCube()
        {
            if (cubes.ContainsKey(Player_UID))
                return cubes[Player_UID];
            else return null;
        }

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
