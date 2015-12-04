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
        private bool allow_merge;
        private double momentum_decay;
        private double momentum_x;
        private double momentum_y;




        [JsonConstructor]
        public Cube(double loc_x, double loc_y, int argb_color, int uID, int team_id, bool food, string name, double mass)
        {
            this.UID = uID;
            this.X = (int)loc_x;
            this.Y = (int)loc_y;
            this.Color = argb_color;
            this.Name = name;
            this.Food = food;
            this.Mass = (int)mass;
            this.Width = (int)(Math.Sqrt(this.Mass));
            this.allow_merge = true;
            this.team_id = uID;
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

        /// <summary>
        /// Helper for returning color of a cube
        /// </summary>
        /// <returns></returns>
        public int GetColor()
        {
            return this.Color;
        }

        /// <summary>
        /// Equals method to compare two cubes
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool Equals(Cube c)
        {
            return (this.UID == c.UID);
        }

        /// <summary>
        /// Equals method to see if an object is a cube
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (((Cube)obj) == null)
            {
                return false;
            }
            return (this.UID == ((Cube)obj).UID);
        }

        /// <summary>
        /// Overrides hashcode to return uid as the hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.UID;
        }

        public void set_momentum(double momentum_x, double momentum_y, int steps)
        {
            this.momentum_x = momentum_x;
            this.momentum_y = momentum_y;
            this.momentum_decay = steps;
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
            MAX_FOOD = 5000,
            MINIMUM_ATTRITION_MASS = 700,
            MIN_FAST_ATTRITION_MASS = 1100,
            MINIMUM_SPLIT_MASS = 100,
            MAXIMUM_SPLIT_DISTANCE = 50,
            MAXIMUM_SPLITS = 6,
            SPLIT_INTERVAL = 10;

        public readonly double ABSORB_DISTANCE_DELTA = 0.25,
            ATTRITION_RATE = 1.25,
            FAST_ATTRITION_RATE = 2.5;

        //TODO: Change this to green
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
        //public Dictionary<int, Cube> cubes = new Dictionary<int, Cube>();
        //Keeps track of all player cubes
        public Dictionary<int, Cube> player_cubes = new Dictionary<int, Cube>();

        //Keeps track of all the food cubes 
        public Dictionary<int, Cube> food_cubes = new Dictionary<int, Cube>();
        //Keeps track of all the virus cubes
        public Dictionary<int, Cube> virus_cubes = new Dictionary<int, Cube>();
        //Keeps track of all split cubes
        public Dictionary<int, LinkedList<Cube>> split_player = new Dictionary<int, LinkedList<Cube>>();


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
                                    case "split_interval":
                                        this.SPLIT_INTERVAL = reader.ReadElementContentAsInt();
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
            Random random = new Random();
            Cube cube = new Cube(RandomX(), RandomY(), Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)).ToArgb(), 0, 0, false, name, PLAYER_START_MASS);
            GetNextUID(cube);
            player_cubes.Add(cube.UID, cube);
            return cube;

        }

        public void GetNextUID(Cube c)
        {
            int result = UIDGenerator.Next(MAX_UID);
            if (c.IsPlayer())
            {
                if (player_cubes.ContainsKey(result))
                {
                    while (player_cubes.ContainsKey(result))
                    {
                        result = UIDGenerator.Next(MAX_UID);
                    }
                }
            }
            c.UID = result;
            if (c.IsFood())
            {
                if (food_cubes.ContainsKey(result))
                {
                    while (food_cubes.ContainsKey(result))
                    {
                        result = UIDGenerator.Next(MAX_UID);
                    }
                }
            }
            c.UID = result;

        }

        public Cube GetPlayerCube(int UID)
        {
            return player_cubes.ContainsKey(UID) ? player_cubes[UID] : null;
        }

        public Cube GetFoodCube(int UID)
        {
            return food_cubes.ContainsKey(UID) ? food_cubes[UID] : null;
        }

        public Cube GetVirusCube(int UID)
        {
            return virus_cubes.ContainsKey(UID) ? virus_cubes[UID] : null;
        }

        /// <summary>
        /// Return a cube based on its UID
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        //public Cube GetCube(int UID)
        //{
        //    if (cubes.ContainsKey(UID))
        //        return cubes[UID];
        //    else return null;
        //}

        public Cube AddFoodCube()
        {
            lock (this)
            {
                if (food_cubes.Keys.Count < MAX_FOOD)
                {
                    Cube food = new Cube(RandomX(), RandomY(), Color.FromArgb(randomX.Next(int.MaxValue)).ToArgb(), 0, 0, true, "", FOOD_VALUE);
                    GetNextUID(food);
                    food_cubes.Add(food.UID, food);
                    return food;
                }
                return null;
            }
        }

        /// <summary>
        /// Adds a virus cube to the world. The cube color will be green, and if
        /// any player cube touches it, the player cube will explode and die.
        /// </summary>
        /// <returns></returns>
        //public void AddVirusCube()
        //{
        //    Cube virus = new Cube(RandomX(), RandomY(), VIRUS_COLOR, GetNextUID(), 0, true, "", VIRUS_MASS);
        //    ProcessCube(virus);
        //}

        public void ProcessData(string type, int x, int y, int player_uid)
        {
            //Move request sent
            if (type.Equals("move"))
            {
                ProcessMove(x, y, player_uid);
            }
            else if (type.Equals("split"))
            {
                SetupSplitCube(player_uid, x, y);
            }

        }

        /// <summary>
        /// Looks at the string and determines the location x, y where the cube wants to move
        /// </summary>
        /// <param name="moveRequest"></param>
        private void ProcessMove(int x, int y, int player_uid)
        {
            Cube player = player_cubes[player_uid];
            ProcessPlayerMovement(x, y, player);
            WorldsEdgeHandler(player);

            ProcessCube(player);
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
        /// Method for taking a cube and splitting it into another cube, with momentum added
        /// when the cubes split
        /// TODO: Getting it to merge
        /// </summary>
        /// <param name="player_uid"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetupSplitCube(int player_uid, double x, double y)
        {
            
            Cube player_cube = player_cubes[player_uid];
            if (player_cube.Mass >= MINIMUM_SPLIT_MASS)
            {
                LinkedList<Cube> split_pieces = new LinkedList<Cube>();
                LinkedList<Cube> buffer = new LinkedList<Cube>();

                split_pieces.AddFirst(player_cube);
                split_player.Add(player_cube.team_id, split_pieces);

                LinkedList<Cube> split_up = new LinkedList<Cube>();
                foreach (Cube cube in split_pieces)
                {
                    cube.Mass /= 2;
                    Cube new_cube = new Cube(cube.X, cube.Y, cube.Color, 0, 0, false, cube.Name, cube.Mass);
                    GetNextUID(new_cube);
                    new_cube.team_id = cube.team_id;

                    double distance = Math.Sqrt((x - cube.X) * (x - cube.X) + (y - cube.Y) * (y - cube.Y));
                    double momentum_width = 25;
                    new_cube.set_momentum(((x-cube.X)/distance)*momentum_width, ((y-cube.Y)/distance)*momentum_width, HEARTBEATS_PER_SECOND);
                    buffer.AddFirst(new_cube);
                    this.player_cubes[new_cube.UID] = new_cube;
                }

                foreach(Cube c in buffer)
                {
                    split_pieces.AddFirst(c);
                }
            }
        }




        /// <summary>
        /// This takes care of what we need to do with a cube, depending on whether it exists and if it has mass.
        /// </summary>
        /// <param name="cube"></param>
        public void ProcessCube(Cube cube)
        {
            bool cubeExists = false;
            if (player_cubes.ContainsKey(cube.UID) || food_cubes.ContainsKey(cube.UID))
            {
                cubeExists = true;
            }
            bool IsVirus = cube.IsVirus();
            bool IsFood = cube.Food;
            bool IsPlayer = !cube.Food && !cube.IsVirus();

            if (cubeExists)
            {
                //If mass is 0, that means the cube has been eaten/destroyed, and should no longer be shown.
                if (cube.Mass == 0)
                {
                    //cubes.Remove(cube.UID);
                    if (IsVirus) virus_cubes.Remove(cube.UID);
                    else if (IsFood) food_cubes.Remove(cube.UID);
                    else if (IsPlayer) player_cubes.Remove(cube.UID);
                }

                //If the mass isn't 0, it means the cube exists, and should be updated. The location may change, size, etc.
                else
                {
                    if (IsPlayer) player_cubes[cube.UID] = cube;
                    if (IsVirus) virus_cubes[cube.UID] = cube;
                    else if (IsFood) food_cubes[cube.UID] = cube;
                }
            }

            //If the cube doesn't exist, only add it if it has a mass.
            else
            {
                if (cube.Mass > 0)
                {
                    //cubes.Add(cube.UID, cube);
                    if (IsVirus) virus_cubes.Add(cube.UID, cube);
                    else if (IsFood) food_cubes.Add(cube.UID, cube);
                    else if (IsPlayer) player_cubes.Add(cube.UID, cube);
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
                foreach (Cube c in player_cubes.Values)
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

        public LinkedList<Cube> FoodConsumed()
        {

            LinkedList<Cube> eaten_cubes = new LinkedList<Cube>();
            foreach (Cube player in player_cubes.Values)
            {
                foreach (Cube food in food_cubes.Values)
                {
                    if (AreOverlapping(player, food))
                    {
                        eaten_cubes.AddFirst(food);
                        player.Mass += food.Mass;
                        food.Mass = 0.0;
                    }
                }
                lock (locker)
                {
                    foreach (Cube eaten_cube in eaten_cubes)
                    {
                        this.food_cubes.Remove(eaten_cube.UID);
                        //this.cubes.Remove(cube3.UID);
                    }
                }
            }
            return eaten_cubes;
        }




        /// <summary>
        /// This is the main method for updating the world, which is done every
        /// heartbeat. Check the players against the food cubes, and see if
        /// any of them need to be removed/changed.
        /// </summary>
        public void Update()
        {
            HashSet<Cube> playersToUpdate = new HashSet<Cube>();
            HashSet<Cube> foodToUpdate = new HashSet<Cube>();
            LinkedList<Cube> virusToUpdate = new LinkedList<Cube>();

            foreach (Cube player in player_cubes.Values)
            {
                lock (locker)
                {
                    foreach (Cube cube in food_cubes.Values)
                    {
                        if (AreOverlapping(player, cube))
                        {
                            if (cube.IsFood())
                            {
                                player.Mass += cube.Mass;
                                cube.Mass = 0;
                                playersToUpdate.Add(player);
                                foodToUpdate.Add(cube);
                            }
                            else if (cube.IsVirus())
                            {
                                player.Mass = 0;
                                playersToUpdate.Add(player);
                            }
                            else if (cube.IsPlayer())
                            {
                                if (player.Mass > cube.Mass)
                                {
                                    player.Mass += cube.Mass;
                                    cube.Mass = 0;
                                    playersToUpdate.Add(player);
                                    foodToUpdate.Add(cube);
                                }
                            }

                        }
                    }

                }
            }

            //Now, process the change buffers
            foreach (Cube c in foodToUpdate)
            {
                ProcessCube(c);
            }

            foreach (Cube c in playersToUpdate)
            {
                ProcessCube(c);
            }

            foreach (Cube c in virusToUpdate)
            {
                ProcessCube(c);
            }
        }

        /// <summary>
        /// See what cubes are overlapping so we can know which food to mark as "eaten".
        /// Note: We're effectively expanding the player cube in this context so we can absorb
        /// more food cubes. We were doing this without the expansion before, but weren't getting
        /// enough of the food cubes absorbed, so we needed to make that update here.
        /// </summary>
        /// <param name="cube1"></param>
        /// <param name="cube2"></param>
        /// <returns></returns>
        private bool AreOverlapping(Cube cube1, Cube cube2)
        {
            int left = (int)Math.Max(cube1.Left - cube1.Width, cube2.Left);
            int right = (int)Math.Min(cube1.Right + cube1.Width, cube2.Right);
            int top = (int)Math.Max(cube1.Top - cube1.Width, cube2.Top);
            int bottom = (int)Math.Min(cube1.Bottom + cube1.Width, cube2.Bottom);
            int width = right - left;
            int height = bottom - top;

            return (width > 0 && height > 0);
        }
    }
}
