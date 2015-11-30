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

        public int Left
        {
            get
            {
                return X;
            }
        }

        public int Right
        {
            get
            {
                return X + Width;
            }
        }

        public int Top
        {
            get
            {
                return Y;
            }
        }

        public int Bottom
        {
            get
            {
                return Y + Width;
            }
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
        /// This returns a "copy" of the cube you're passing in
        /// </summary>
        /// <param name="cube"></param>
        /// <returns></returns>
        public static Cube Copy(Cube cube)
        {
            return new Cube(cube.X, cube.Y, cube.Color, cube.UID, cube.team_id, cube.Food, cube.Name, cube.Mass);
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
        public readonly int WIDTH = 1000, HEIGHT = 1000, HEARTBEATS_PER_SECOND = 20, TOP_SPEED = 5, LOW_SPEED = 1, FOOD_VALUE = 5, PLAYER_START_MASS = 1000, MAX_FOOD = 500, MINIMUM_SPLIT_MASS = 200, MAXIMUM_SPLIT_DISTANCE = 50, MAXIMUM_SPLITS = 6;
        public readonly double ABSORB_DISTANCE_DELTA = 0.25, ATTRITION_RATE = 1.25;

        //TODO: Change this to green
        public readonly static int VIRUS_COLOR = 0;
        public readonly static int VIRUS_MASS = 500;

        public double Scale = 2.0;
        public string Player_Name;
        public int Player_Start_Mass = 1000;
        public int Player_UID;
        public int xoff, yoff;
        private int split_count = 0;
        
        public Dictionary<int, Cube> cubes = new Dictionary<int, Cube>();

        private string gameplay_file = "world_parameters.xml";
        
        public World()
        {

        }

        /// <summary>
        /// Reads the gameplay parameters file when creating a new world.
        /// In order for this to work, the gameplay file needs to be called
        /// "world_parameters.xml" and live in the same place in the filesystem 
        /// as the server.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public World(string filename)
        {
            if (gameplay_file.Equals(filename))
            {
                if (System.IO.File.Exists(filename))
                {
                    //Try reading the XML file, and inputting those parameters into our gameplay environment
                    using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(filename))
                    {
                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {
                                switch (reader.Name)
                                {
                                    case "width":
                                        this.WIDTH = reader.ReadElementContentAsInt();
                                       break;
                                    case "height":
                                        this.HEIGHT = reader.ReadElementContentAsInt();
                                        break;
                                    case "max_splits":
                                        this.MAXIMUM_SPLITS = reader.ReadElementContentAsInt();
                                        break;
                                    case "max_split_distance":
                                        this.MAXIMUM_SPLIT_DISTANCE = reader.ReadElementContentAsInt();
                                        break;
                                    case "top_speed":
                                        this.TOP_SPEED = reader.ReadElementContentAsInt();
                                        break;
                                    case "low_speed":
                                        this.LOW_SPEED = reader.ReadElementContentAsInt();
                                        break;
                                    case "attrition_rate":
                                        this.ATTRITION_RATE = reader.ReadElementContentAsDouble();
                                        break;
                                    case "food_value":
                                        this.FOOD_VALUE = reader.ReadElementContentAsInt();
                                        break;
                                    case "player_start_mass":
                                        this.PLAYER_START_MASS = reader.ReadElementContentAsInt();
                                        break;
                                    case "max_food":
                                        this.MAX_FOOD = reader.ReadElementContentAsInt();
                                        break;
                                    case "min_split_mass":
                                        this.MINIMUM_SPLIT_MASS = reader.ReadElementContentAsInt();
                                        break;
                                    case "absorb_constant":
                                        this.ABSORB_DISTANCE_DELTA = reader.ReadElementContentAsDouble();
                                        break;
                                    case "heartbeats_per_second":
                                        this.HEARTBEATS_PER_SECOND = reader.ReadElementContentAsInt();
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// This is specific to adding the player cube so that we can save 
        /// some of the data about the cube (player mass, name, uid, etc.)
        /// </summary>
        /// <param name="json"></param>
        public void AddPlayerCube(string json)
        {
            Cube cube = Cube.Create(json);
            ProcessCube(cube);
            Player_Start_Mass = PLAYER_START_MASS;
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

        /// <summary>
        /// This helper method helps us to know what to do with the cubes that are within eating distance
        /// of the player cube, based on the absorb constant. 
        /// 
        /// Here are a few of the scenarios that can happen here:
        /// 1)  If the cube is a virus, the player cube dies and the game is over.
        /// 2)  If the cube is food, the player cube eats it, thereby increasing the player cube's mass and destroying the food cube.
        /// 3)  If the cube is another player, the bigger player cube eats the smaller player cube, and the smaller player cube dies 
        /// </summary>
        public void ProcessCubesInPlayerSpace()
        {
            Cube player_cube = GetPlayerCube();
            foreach(Cube cube in cubes.Values)
            {
                if (ReferenceEquals(player_cube, cube)) continue;
                if(CollisionDetected(player_cube, cube))
                {
                    //If the cube is a virus, set the player cube mass to 0 (it died)
                    if(cube.Color == World.VIRUS_COLOR)
                    {
                        player_cube.Mass = 0;
                        break;
                    }

                    //If the cube is food
                    if(cube.Food == true)
                    {
                        player_cube.Mass += cube.Mass;
                        cube.Mass = 0;
                    }

                    //If the cube is another player
                    if(cube.Food == false)
                    {
                        if(cube.Mass > player_cube.Mass)
                        {
                            cube.Mass += player_cube.Mass;
                            player_cube.Mass = 0;
                        }
                        else
                        {
                            player_cube.Mass += cube.Mass;
                            cube.Mass = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This is a helper method that helps us determine if there was a collision with
        /// a player cube.
        /// The way this works is we determine the area of the cubes that are overlapping.
        /// If the area that's overlapping compared to the area of the non-player cube
        /// is greater than the absorb delta, then there's a collision.
        /// Otherwise, there's no collision.
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="player_cube"></param>
        /// <returns></returns>
        private bool CollisionDetected(Cube player_cube, Cube cube)
        {
            double overlap = OverlappingArea(player_cube, cube);
            double delta = overlap / cube.Mass;

            return delta > ABSORB_DISTANCE_DELTA;
        }

        /// <summary>
        /// This helper method helps us to determine how much area
        /// is overlapped between two cubes.
        /// </summary>
        /// <param name="cube1"></param>
        /// <param name="cube2"></param>
        /// <returns></returns>
        private double OverlappingArea(Cube cube1, Cube cube2)
        {
            double left = Math.Max(cube1.Left, cube2.Left);
            double right = Math.Min(cube1.Right, cube2.Right);
            double top = Math.Max(cube1.Top, cube2.Top);
            double bottom = Math.Min(cube1.Bottom, cube2.Bottom);

            double width = Math.Max(0, right - left);
            double height = Math.Max(0, bottom - top);
            return width * height;
        }

        /// <summary>
        /// If the world has received a "split" request, check to see if it's valid (we haven't reached our max number of splits, for example).
        /// If it's valid, process the request by splitting the cube into a new cube with the same team ID. If it's not valid, do nothing.
        /// </summary>
        /// <param name="cube">The cube to split</param>
        /// <param name="dest_x">The x location to split towards</param>
        /// <param name="dest_y">The y location to split towards</param>
        public void ProcessSplit(Cube cube, int dest_x, int dest_y)
        {
            if(split_count < MAXIMUM_SPLITS && cube.Mass >= MINIMUM_SPLIT_MASS)
            {
                Cube new_cube = Cube.Copy(cube);
                new_cube.Mass = cube.Mass /= 2;
                new_cube.team_id = cube.team_id = cube.UID;

                //Set the new x and y coordinates for the split cubes
                
            }
        }
    }
}
