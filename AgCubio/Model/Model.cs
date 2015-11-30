﻿using System;
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
        public readonly int WIDTH = 1000, HEIGHT = 1000, HEARTBEATS_PER_SECOND = 20, TOP_SPEED = 5, LOW_SPEED = 1, ATTRITION_RATE = 1, FOOD_VALUE = 5, PLAYER_START_MASS = 1000, MAX_FOOD = 500, MINIMUM_SPLIT_MASS = 200, MAXIMUM_SPLIT_DISTANCE = 50, MAXIMUM_SPLITS = 6;
        public readonly double ABSORB_DISTANCE_DELTA = 0.25;

        //TODO: Change this to green
        public readonly static int VIRUS_COLOR = 0;
        public readonly static int VIRUS_MASS = 500;

        public double Scale = 2.0;
        public string Player_Name;
        public int Player_Start_Mass = 1000;
        public int Player_UID;
        public int xoff, yoff;
        
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
                        reader.ReadStartElement();
                        this.WIDTH = 1000;
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
        /// Getter for the width of the world
        /// </summary>
        /// <returns></returns>
        public int GetWidth()
        {
            return WIDTH;
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
