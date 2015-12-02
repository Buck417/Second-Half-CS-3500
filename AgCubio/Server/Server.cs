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
using System.Timers;

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
            Timer t = new Timer();
            t.Interval = 1000 / world.HEARTBEATS_PER_SECOND;
            t.Elapsed += HeartBeatElapsed;
        }

        /// <summary>
        /// This function is the heartbeat of this program - it is literally the pulse
        /// of the AgCubio server. We want to analyze our world and send it in this function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HeartBeatElapsed(object sender, ElapsedEventArgs e)
        {
            //Analyze world

            //Send world

        }

        //Start


        //Handle new client connections
        private static void Handle_New_Client_Connections(Preserved_State state)
        {
            state.callback = new Action<Preserved_State>(Receive_Player_Name);
            Network.i_want_more_data(state);
        }

        /// <summary>
        /// Once the player name is received, create the player cube and add some food
        /// cubes. Once all the cubes are added, send them to the client.
        /// Make sure the callback is changed to HandleData, and ask for more data
        /// from the client.
        /// </summary>
        /// <param name="state"></param>
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
        /// Helper method for sending the world data to 
        /// the client.
        /// </summary>
        /// <param name="state"></param>
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

        /// <summary>
        /// Handle incoming data from the client, i.e. move and split
        /// requests. From here, process that data as it relates to the world
        /// and then send the world back to the client.
        /// </summary>
        /// <param name="state"></param>
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
                //Calculate distance based on speed (rate = speed / heartbeat)
                double speed = world.TOP_SPEED;
                double rate = ((speed / ((double)world.HEARTBEATS_PER_SECOND)) * ((double)world.PLAYER_START_MASS / (double)player.Mass)) / 5.0;
                if (player.X != x) player.X += (int)(rate * (x - player.X));
                if(player.Y != y) player.Y += (int)(rate * (y - player.Y));
                world.ProcessCube(player);
            }
        }

        static void ProcessSplit(string splitRequest)
        {
            splitRequest = Regex.Replace(splitRequest.Trim(), "[()]", "");
            string[] split = splitRequest.Split('\n');
            if (split.Length < 2) return;
            string splitArgs = split[split.Length - 1];

            int x, y;
            string[] positions = splitArgs.Split(',');
            if(int.TryParse(positions[1], out x) && int.TryParse(positions[2], out y))
            {
                if(world.split_count > world.MAXIMUM_SPLITS)
                {
                    return;
                }
                Cube player = world.GetPlayerCube();
                Cube split_cube = Cube.Copy(player);
                world.AssignUID(split_cube);
                player.Mass = split_cube.Mass /= 2;

                //Now that we have the two cubes, figure out where to put them
                //Move one to the left by max split / 2, and the other to the right by that much
                player.X -= (world.MAXIMUM_SPLIT_DISTANCE / 2);
                split_cube.X += (world.MAXIMUM_SPLIT_DISTANCE / 2);
            }
        }



        /********************************** END HANDLE GAMEPLAY MECHANICS ***********************/
    }
}
