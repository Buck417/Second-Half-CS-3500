﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Network_Controller;
using System.Drawing;
using System.Text.RegularExpressions;
using Newtonsoft.Json;



namespace Server
{
    class AgServer
    {
        public static World world;

        public static void Main(string[] args)
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
            string player_name = Regex.Replace(state.sb.ToString().Trim(), @"\n|\t|\r", "");
            Cube player_cube = world.AddPlayerCube(player_name);

            //Sends the player cube and starting food cubes to the client
            lock (world)
            {
                //Network.Send(state.socket, "{\"loc_x\":500.0,\"loc_y\":600.0,\"argb_color\":-65536,\"uid\":5571,\"team_id\":5571,\"food\":false,\"Name\":\"" + player_name + "\",\"Mass\":900.0}");
                Network.Send(state.socket, JsonConvert.SerializeObject(player_cube) + "\n");
                StringBuilder string_builder = new StringBuilder();
                
                foreach (Cube cube in world.food_cubes.Values)
                {
                    string_builder.Append(JsonConvert.SerializeObject(cube) + "\n");
                }
            }
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

        

        /********************************** END HANDLE GAMEPLAY MECHANICS ***********************/
    }
}
