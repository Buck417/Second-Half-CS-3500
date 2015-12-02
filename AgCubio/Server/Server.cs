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

        static void SendWorld(Preserved_State state)
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
            
            ProcessData(str);
            //Update the world and send it back
            SendWorld(state);

            Network.i_want_more_data(state);
        }

        static void ProcessData(string data)
        {
            lock (world)
            {
                //Move request sent
                if (data.IndexOf("move") != -1)
                {
                    ProcessMove(data);
                }
                if (data.IndexOf("split") != -1)
                {
                    ProcessSplit(data);
                }
                world.ProcessCubesInPlayerSpace();
            }
        }
        
        /*********************************** HANDLE NETWORK COMMUNICATIONS **********************/

        /********************************* END HANDLE NETWORK COMMUNICATIONS ********************/




        /************************************ HANDLE GAMEPLAY MECHANICS *************************/
        static void ProcessMove(string moveRequest)
        {

            moveRequest = Regex.Replace(moveRequest.Trim(), "[()]", "");
            string[] move = moveRequest.Split('\n');
            if (move.Length < 2) return;
            string moveArgs = move[move.Length - 1];

            int x, y;
            string[] positions = moveArgs.Split(',');
            if (int.TryParse(positions[1], out x) && int.TryParse(positions[2], out y))
            {
                Cube player = world.GetPlayerCube();
                player.X = x;
                player.Y = y;
                world.ProcessCube(player);
            }
        }

        static void ProcessSplit(string splitRequest)
        {

        }



        /********************************** END HANDLE GAMEPLAY MECHANICS ***********************/
    }
}
