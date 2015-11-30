﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Network_Controller;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Server
{
    class AgServer
    {
        public static World world;

        static void Main(string[] args)
        {
            AgServer server = new AgServer();
            world = new World();
            Network.Server_Awaiting_Client_Loop(Handle_New_Client_Connections);
        }

        //Start


        //Handle new client connections
        static void Handle_New_Client_Connections(IAsyncResult ar)
        {
            Preserved_State state = (Preserved_State)ar.AsyncState;
            state.callback = Receive_Player_Name;
            Network.i_want_more_data(ar);
        }

        //Receive the player name
        static void Receive_Player_Name(IAsyncResult ar)
        {
            Preserved_State state = (Preserved_State)ar.AsyncState;
            state.callback = HandleData;

            //Generates the data from the json and adds the player cube to the world
            string playerName = Regex.Replace(state.sb.ToString().Trim(), @"\n|\t|\r", "");
            world.AddPlayerCube(playerName);

            //Send the player name
            Network.Send(state.socket, "{\"loc_x\":500.0,\"loc_y\":600.0,\"argb_color\":-65536,\"uid\":5571,\"team_id\":5571,\"food\":false,\"Name\":\"" + state.sb.ToString() + "\",\"Mass\":900.0}");
            
            Network.i_want_more_data(ar);
        }

        //Handle data from the client
        static void HandleData(IAsyncResult ar)
        {
            Preserved_State state = (Preserved_State)ar.AsyncState;
            string str = state.sb.ToString();

            ProcessData(str);
        }

        static void ProcessData(string data)
        {
            //Move request sent
            if(data.IndexOf("move") != -1)
            {
                ProcessMove(data);
            }
            if(data.IndexOf("split") != -1)
            {
                ProcessSplit(data);
            }
        }

        //Update
        //TODO: Update the world
        static void ProcessMove(string moveRequest)
        {

        }

        static void ProcessSplit(string splitRequest)
        {

        }
        /*********************************** HANDLE NETWORK COMMUNICATIONS **********************/

        /********************************* END HANDLE NETWORK COMMUNICATIONS ********************/




        /************************************ HANDLE GAMEPLAY MECHANICS *************************/
        /// <summary>
        /// This method handles the split request.
        /// </summary>
        /// <returns>If the request isn't handled properly, or if an error
        /// occurs, return false. Otherwise, return true.</returns>
        private bool HandleSplitRequest()
        {
            bool result = false;

            return result;
        }

        /// <summary>
        /// Adds a food cube to the world. The cube color will be random,
        /// as long as it's not green - those are for viruses only.
        /// </summary>
        /// <returns>If there are any errors, return false. Otherwise, return true.</returns>
        private bool AddFoodCube()
        {
            //TODO: Determine when to begin adding food cubes for a new world
            //TODO: Get better values in for the Cube constructor
            Random random = new Random();
            int food_count = 0;

            //Check if food should be added
            if (world.cubes.Count < World.max_food)
            {
                //Check if alot of food should be added, like the start of the game
                if (world.cubes.Count < 10)
                {
                    lock (this)
                    {
                        while (food_count < 100)
                        {
                            Cube food = new Cube((double)random.Next(0, world.width), (double)random.Next(0, world.width), Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)).ToArgb(), 0, 0, true, "", 1.0);
                            world.cubes.Add(food.team_id, food);

                        }
                        return true;
                    }
                }
                //If the world needs only a little more food, add here
                lock (this)
                {
                    Cube food = new Cube((double)random.Next(0, world.width), (double)random.Next(0, world.width), Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)).ToArgb(), 0, 0, true, "", 1.0);
                    world.cubes.Add(food.team_id, food);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds a virus cube to the world. The cube color will be green, and if
        /// any player cube touches it, the player cube will explode and die.
        /// </summary>
        /// <param name="x">The x coordinate of the new virus</param>
        /// <param name="y">The y coordinate of the new virus</param>
        /// <returns></returns>
        private bool AddVirusCube(int x, int y)
        {
            bool result = false;
            
            int uid = GetNextUID();
            int team_id = GetNextUID();
            Cube virus = new Cube(x, y, World.VIRUS_COLOR, uid, team_id, true, "", World.VIRUS_MASS);
            world.ProcessCube(virus);

            return result;
        }
        
        /// <summary>
        /// This helper method gets us the next user ID we need (make sure it doesn't currently exist!)
        /// </summary>
        /// <returns></returns>
        private int GetNextUID()
        {
            return 1;
        }


        /********************************** END HANDLE GAMEPLAY MECHANICS ***********************/
    }
}
