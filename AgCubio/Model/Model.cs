using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Drawing;
using System.Text.RegularExpressions;

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
        public double X, Y;
        [JsonProperty]
        public int Color;
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public bool Food;
        [JsonProperty]
        public double Mass;
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

        public double Left
        {
            get
            {
                return X;
            }
        }

        public double Right
        {
            get
            {
                return X + Width;
            }
        }

        public double Top
        {
            get
            {
                return Y;
            }
        }

        public double Bottom
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
            try
            {
                return JsonConvert.DeserializeObject<Cube>(json);
            }
            catch (Exception e)
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
        public double GetCenterX()
        {
            return X + (Width / 2);
        }

        /// <summary>
        /// Helper method for getting the center y position of the cube
        /// </summary>
        /// <returns></returns>
        public double GetCenterY()
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

        /// <summary>
        /// Helper for seeing if a cube is a food cube or not
        /// </summary>
        /// <returns></returns>
        public bool IsFood()
        {
            if (this.Food == true)
            {
                return true;
            }
            return false;
        }

        public bool IsPlayer()
        {
            return this.Food == false && !IsVirus();
        }
    }

    /// <summary>
    /// This is basically where all the cubes live, as well as the max dimensions for the world.
    /// </summary>
    public class World
    {
        /****************** CONSTANTS FOR SERVER *************************/
        public readonly int WIDTH = 1000,
            HEIGHT = 1000,
            HEARTBEATS_PER_SECOND = 30,
            TOP_SPEED = 5,
            LOW_SPEED = 1,
            FOOD_VALUE = 5,
            PLAYER_START_MASS = 1000,
            MAX_FOOD = 500,
            MINIMUM_ATTRITION_MASS = 200,
            MIN_FAST_ATTRITION_MASS = 1100,
            MINIMUM_SPLIT_MASS = 100,
            MAXIMUM_SPLIT_DISTANCE = 50,
            MAXIMUM_SPLITS = 6;

        public readonly double ABSORB_DISTANCE_DELTA = 0.25,
            ATTRITION_RATE = 1.25,
            FAST_ATTRITION_RATE = 2.5;

        public readonly static int VIRUS_COLOR = Color.Green.ToArgb();
        public readonly static int VIRUS_MASS = 500;
        public readonly int VIRUS_COUNT = 2;

        private readonly int MAX_UID = 1000000;

        public double Scale = 2.0;
        private int split_count = 0;
        private Random randomX = new Random(1000);
        private Random randomY = new Random(2500);
        private Random UIDGenerator = new Random();

        //Keeps track of ALL cubes
        public Dictionary<int, Cube> cubes = new Dictionary<int, Cube>();
        //Keeps track of all player cubes
        public HashSet<Cube> player_cubes = new HashSet<Cube>();

        //Keeps track of all the food cubes 
        public Dictionary<int, Cube> food_cubes = new Dictionary<int, Cube>();
        //Keeps track of all the virus cubes
        public Dictionary<int, Cube> virus_cubes = new Dictionary<int, Cube>();

        private string gameplay_file = "world_parameters.xml";

        static readonly object locker = new object();


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
                                    case "min_attrition_mass":
                                        this.MINIMUM_ATTRITION_MASS = reader.ReadElementContentAsInt();
                                        break;
                                    case "min_fast_attrition":
                                        this.MIN_FAST_ATTRITION_MASS = reader.ReadElementContentAsInt();
                                        break;
                                    case "fast_attrition_rate":
                                        this.FAST_ATTRITION_RATE = reader.ReadElementContentAsDouble();
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
        /// Generates a random position for cubes
        /// </summary>
        /// <returns></returns>
        private int RandomX()
        {
            return randomX.Next(WIDTH);
        }

        private int RandomY()
        {
            return randomY.Next(HEIGHT);
        }

        /// <summary>
        /// Creates a player cube from the name and adds that cube to the world dictionaries.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The player cube</returns>
        public Cube AddPlayerCube(string name)
        {
            lock (this)
            {

                int UID = GetNextUID();
                Random random = new Random();
                Cube cube = new Cube(RandomX(), RandomY(), Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)).ToArgb(), UID, 0, false, name, PLAYER_START_MASS);

                cubes.Add(cube.UID, cube);
                player_cubes.Add(cube);
                return cube;
            }
        }

        public int GetNextUID()
        {
            int result = UIDGenerator.Next(MAX_UID);
            if (cubes.ContainsKey(result))
            {
                while (cubes.ContainsKey(result))
                {
                    result = UIDGenerator.Next(MAX_UID);
                }
            }
            return result;
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

        public void AddFoodCube()
        {
            lock (this)
            {
                if (food_cubes.Keys.Count < MAX_FOOD)
                {
                    Cube food = new Cube(RandomX(), RandomY(), Color.FromArgb(randomX.Next(int.MaxValue)).ToArgb(), GetNextUID(), 0, true, "", FOOD_VALUE);
                    ProcessCube(food);
                }
            }
        }

        /// <summary>
        /// Adds a virus cube to the world. The cube color will be green, and if
        /// any player cube touches it, the player cube will explode and die.
        /// </summary>
        /// <returns></returns>
        public void AddVirusCube()
        {
            Cube virus = new Cube(RandomX(), RandomY(), VIRUS_COLOR, GetNextUID(), 0, true, "", VIRUS_MASS);
            ProcessCube(virus);
        }

        public void ProcessData(string data)
        {
            //Move request sent
            if (data.IndexOf("move") != -1)
            {
                ProcessMove(data);
            }
            if (data.IndexOf("split") != -1)
            {
                //ProcessSplit(data);
            }

        }

        /// <summary>
        /// Looks at the string and determines the location x, y where the cube wants to move
        /// </summary>
        /// <param name="moveRequest"></param>
        public void ProcessMove(string moveRequest)
        {
            moveRequest = Regex.Replace(moveRequest.Trim(), "[()]", "");
            string[] move = moveRequest.Split('\n');
            string moveArgs;
            if (move.Length >= 2)
                moveArgs = move[move.Length - 1];
            else
                moveArgs = move[0];

            double x, y;
            string[] positions = moveArgs.Split(',');
            if (double.TryParse(positions[1], out x) && double.TryParse(positions[2], out y))
            {
                Cube player = player_cubes.First();
                ProcessPlayerMovement(x, y, player);
                WorldsEdgeHandler(player);

                ProcessCube(player);
            }
        }

        /// <summary>
        /// Handles generating a location for the cube to be drawn at next, giving the impression
        /// that it is moving to its next point at a certain speed, which decreases when the mass
        /// of the player cube is increased.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private Cube ProcessPlayerMovement(double x, double y, Cube player)
        {
            double speed = TOP_SPEED;
            double rate = ((speed / ((double)HEARTBEATS_PER_SECOND)) * ((double)PLAYER_START_MASS / (double)player.Mass)) / 5.0;
            if (player.X != x) player.X += (int)(rate * (x - player.X));
            if (player.Y != y) player.Y += (int)(rate * (y - player.Y));

            if (speed < LOW_SPEED)
                speed = LOW_SPEED;

            ProcessCube(player);
            return player;
        }


        /// <summary>
        /// Handles drawing cubes at the edge of the world
        /// </summary>
        /// <param name="player"></param>
        private void WorldsEdgeHandler(Cube player)
        {
            if (player.X < 0)
            {
                player.X = 0;
            }

            if (player.Y < 0)
            {
                player.Y = 0;
            }
            if (player.X + player.Width > WIDTH)
            {
                player.X = WIDTH - player.Width;
            }
            if (player.Y + player.Width > HEIGHT)
            {
                player.X = HEIGHT - player.Width;
            }
        }

        /// <summary>
        /// This takes care of what we need to do with a cube, depending on whether it exists and if it has mass.
        /// </summary>
        /// <param name="cube"></param>
        public void ProcessCube(Cube cube)
        {
            bool cubeExists = cubes.ContainsKey(cube.UID);
            bool IsVirus = cube.IsVirus();
            bool IsFood = cube.Food;
            bool IsPlayer = !cube.Food && !cube.IsVirus();

            if (cubeExists)
            {
                //If mass is 0, that means the cube has been eaten/destroyed, and should no longer be shown.
                if (cube.Mass == 0)
                {
                    cubes.Remove(cube.UID);
                    if (IsVirus) virus_cubes.Remove(cube.UID);
                    else if (IsFood) food_cubes.Remove(cube.UID);
                    else if (IsPlayer) player_cubes.Remove(cube);
                }

                //If the mass isn't 0, it means the cube exists, and should be updated. The location may change, size, etc.
                else
                {
                    cubes[cube.UID] = cube;
                    if (IsVirus) virus_cubes[cube.UID] = cube;
                    else if (IsFood) food_cubes[cube.UID] = cube;
                }
            }

            //If the cube doesn't exist, only add it if it has a mass.
            else
            {
                if (cube.Mass > 0)
                {
                    cubes.Add(cube.UID, cube);
                    if (IsVirus) virus_cubes.Add(cube.UID, cube);
                    else if (IsFood) food_cubes.Add(cube.UID, cube);
                    else if (IsPlayer) player_cubes.Add(cube);
                }
            }

        }

        /// <summary>
        /// This is where we process the attrition - each player cube decreases in size
        /// by a certain amount for every heartbeat.
        /// </summary>
        public void ProcessAttrition()
        {
            lock (this)
            {
                foreach (Cube c in player_cubes)
                {
                    //If the cube's mass is between the minimum attrition mass and less than the minimum mass for the fast attrition rate, do the slow attrition rate
                    if (c.Mass > MINIMUM_ATTRITION_MASS && c.Mass < MIN_FAST_ATTRITION_MASS)
                        c.Mass -= ATTRITION_RATE;
                    //If the cube's mass is greater than the minimum mass for fast attrition, use that rate.
                    else if (c.Mass > MIN_FAST_ATTRITION_MASS)
                        c.Mass -= FAST_ATTRITION_RATE;

                    //Otherwise, don't change the mass - because it'll be below the minimum attrition mass
                }
            }
        }

        /// <summary>
        /// This is the main method for updating the world, which is done every
        /// heartbeat. Check the players against the food cubes, and see if
        /// any of them need to be removed/changed.
        /// </summary>
        public void Update()
        {
            lock (this) { 
                foreach (Cube player in player_cubes)
                {
                    lock (locker)
                    {
                        foreach (Cube cube in cubes.Values)
                        {
                            if (AreOverlapping(player, cube))
                            {
                                if (cube.IsFood())
                                {
                                    player.Mass += cube.Mass;
                                    cube.Mass = 0;
                                    ProcessCube(player);
                                    ProcessCube(cube);
                                }
                                else if (cube.IsVirus())
                                {
                                    player.Mass = 0;
                                    ProcessCube(player);
                                }
                                else if (cube.IsPlayer())
                                {
                                    if (player.Mass > cube.Mass)
                                    {
                                        player.Mass += cube.Mass;
                                        cube.Mass = 0;
                                        ProcessCube(player);
                                        ProcessCube(cube);
                                    }
                                }

                            }
                        }
                    }

                }
            }
        }

        private bool AreOverlapping(Cube cube1, Cube cube2)
        {
            int left = (int)Math.Max(cube1.Left, cube2.Left);
            int right = (int)Math.Min(cube1.Right, cube2.Right);
            int top = (int)Math.Max(cube1.Top, cube2.Top);
            int bottom = (int)Math.Min(cube1.Bottom, cube2.Bottom);
            int width = right - left;
            int height = top - bottom;

            return width > 0 && height > 0;
        }
    }
}
