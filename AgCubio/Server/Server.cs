using System;
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
            Network.Server_Awaiting_Client_Loop(new Action<Preserved_State>(Handle_New_Client_Connections));
        }

        //Start


        //Handle new client connections
        private static void Handle_New_Client_Connections(Preserved_State state)
        {
            
            state.callback = new Action<Preserved_State>(Receive_Player_Name);
            Network.i_want_more_data(state);
        }

        //Receive the player name
        private static void Receive_Player_Name(Preserved_State state)
        {
            //Preserved_State state = (Preserved_State)ar.AsyncState;
            state.callback = new Action<Preserved_State>(HandleData);

            //Generates the data from the json and adds the player cube to the world
            string player_name = Regex.Replace(state.sb.ToString().Trim(), @"\n|\t|\r", "");
            Cube player_cube = world.AddPlayerCube(player_name);
            while (world.AddFoodCube())
            {
            }

            //Sends the player cube and starting food cubes to the client
            lock (world)
            {
                Network.Send(state.socket, JsonConvert.SerializeObject(player_cube) + "\n");
                SendWorld(state);
            }
            Network.i_want_more_data(state);
        }
        
        /// <summary>
        /// Creates initial state of the world to be sent to the client
        /// </summary>
        /// <param name="state"></param>
        private static void SendWorld(Preserved_State state)
        {
            StringBuilder string_builder = new StringBuilder();
            lock (world) {
                foreach (Cube cube in world.cubes.Values)
                    {
                        string_builder.Append(JsonConvert.SerializeObject(cube) + "\n");
                    }
                Network.Send(state.socket, string_builder.ToString());
            }
        }

        //Handle data from the client
        static void HandleData(Preserved_State state)
        {
            //Preserved_State state = (Preserved_State)ar.AsyncState;
            string str = state.sb.ToString();
                          
            world.AddFoodCube();

            world.ProcessData(str);
            //Update the world and send it back
            SendWorld(state);

            Network.i_want_more_data(state);
        }

        
        
        /*********************************** HANDLE NETWORK COMMUNICATIONS **********************/

        /********************************* END HANDLE NETWORK COMMUNICATIONS ********************/




        /************************************ HANDLE GAMEPLAY MECHANICS *************************/
       



        /********************************** END HANDLE GAMEPLAY MECHANICS ***********************/
    }
}
