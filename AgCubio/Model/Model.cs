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
            this.Width = (int)(Math.Sqrt(this.Mass));
        }

        /// <summary>
        /// Helper method for creating a cube, based on its JSON representation.
        /// If there's an error with the JSON, write it to the console and return null.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Cube Create(string json)
        {
            try {
                return JsonConvert.DeserializeObject<Cube>(json);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Helper method for getting the center x position of the cube
        /// </summary>
        /// <returns></returns>
        public int GetCenterX()
        {
            return X + (Width / 2);
        }

        /// <summary>
        /// Helper method for getting the center y position of the cube
        /// </summary>
        /// <returns></returns>
        public int GetCenterY()
        {
            return Y + (Width / 2);
        }

        /// <summary>
        /// This helper method helps us tell if the cube is a virus
        /// or not.  Currently, the specification is that the cube is green.
        /// The 
        /// </summary>
        /// <returns></returns>
        public bool IsVirus()
        {
            if (this.Color == World.VIRUS_COLOR) return true;
            else return false;
        }
    }

    /// <summary>
    /// This is basically where all the cubes live, as well as the max dimensions for the world.
    /// </summary>
    public class World
    {
        /****************** CONSTANTS FOR SERVER *************************/
        int WIDTH, HEIGHT, HEARTBEATS_PER_SECOND, TOP_SPEED, LOW_SPEED, ATTRITION_RATE, FOOD_VALUE, PLAYER_START_MASS, MAX_FOOD, MINIMUM_SPLIT_MASS, MAXIMUM_SPLIT_DISTANCE, MAXIMUM_SPLITS;
        double ABSORB_DISTANCE_DELTA;

        //TODO: Change this to green
        public readonly static int VIRUS_COLOR = 0;
        public readonly static int VIRUS_MASS = 500;

        public double Scale = 2.0;
        public string Player_Name;
        public int Player_UID;
        public double Player_Start_Mass;
        public int xoff, yoff;

        public Dictionary<int, Cube> cubes = new Dictionary<int, Cube>();

        /// <summary>
        /// This is specific to adding the player cube so that we can save 
        /// some of the data about the cube (player mass, name, uid, etc.)
        /// </summary>
        /// <param name="json"></param>
        public void AddPlayerCube(string json)
        {
            Cube cube = Cube.Create(json);
            ProcessCube(cube);
            Player_Start_Mass = cube.Mass;
            Player_Name = cube.Name;
            Player_UID = cube.UID;
        }

        /// <summary>
        /// Helper method for just returning the player cube only.
        /// If the player cube isn't in the list of cubes (if it's been destroyed),
        /// then null is returned.
        /// </summary>
        /// <returns></returns>
        public Cube GetPlayerCube()
        {
            if (cubes.ContainsKey(Player_UID))
                return cubes[Player_UID];
            else return null;
        }

        /// <summary>
        /// Get the mass for the player cube
        /// </summary>
        /// <returns></returns>
        public double GetPlayerMass()
        {
            return cubes[Player_UID].Mass;
        }

        /// <summary>
        /// Return a cube based on its UID
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
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
        public void ProcessCube(Cube cube)
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
