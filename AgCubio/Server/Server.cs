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
        public volatile static World world;
        private volatile static System.Net.Sockets.Socket dataSocket;

        private static Timer heartbeatTimer = new Timer();
        private static Timer splitTimer = new Timer();
        private static System.Collections.Concurrent.ConcurrentQueue<Tuple<string, int, int, int>> playerMovementQueue = new System.Collections.Concurrent.ConcurrentQueue<Tuple<string, int, int, int>>();
        
        public static void Main(string[] args)
        {
            AgServer server = new AgServer();
            world = new World("world_parameters.xml");
            splitTimer.Interval = world.SPLIT_INTERVAL;
            splitTimer.Elapsed += SplitTimer_Elapsed;
            Network.Server_Awaiting_Client_Loop(new Action<Preserved_State>(Handle_New_Client_Connections));
        }

        /// <summary>
        /// Once the split timer finishes, make sure the mass goes back to normal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SplitTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void HeartbeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            heartbeatTimer.Stop();

            world.ProcessAttrition();
            StringBuilder string_builder = new StringBuilder();
            LinkedList<Cube> cubes_eaten = world.FoodConsumed();
            Tuple<string, int, int, int> action;
            if (playerMovementQueue.TryDequeue(out action))
            {
                string type = action.Item1;
                int x = action.Item2;
                int y = action.Item3;
                int player_uid = action.Item4;
                world.ProcessData(type, x, y, player_uid);
            }

            Network.Send(dataSocket, JsonConvert.SerializeObject(world.AddFoodCube()) + "\n");

            foreach (Cube cube in cubes_eaten)
            {
                Network.Send(dataSocket, JsonConvert.SerializeObject(cube) + "\n");
            }

            foreach (Cube cube in world.player_cubes.Values)
            {
                Network.Send(dataSocket, JsonConvert.SerializeObject(cube) + "\n");
            }

            heartbeatTimer.Start();
        }

        /*********************************** HANDLE NETWORK COMMUNICATIONS **********************/
        //Handle new client connections
        private static void Handle_New_Client_Connections(Preserved_State state)
        {
            dataSocket = state.socket;
            state.callback = new Action<Preserved_State>(Receive_Player_Name);

            heartbeatTimer.Interval = 1000 / world.HEARTBEATS_PER_SECOND;
            heartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
            heartbeatTimer.Start();
            Network.i_want_more_data(state);
        }

        //Receive the player name
        private static void Receive_Player_Name(Preserved_State state)
        {
            //Preserved_State state = (Preserved_State)ar.AsyncState;
            state.callback = new Action<Preserved_State>(HandleData);
            string playerName = state.sb.ToString();

            Cube player = world.AddPlayerCube(playerName);
            state.SetUID(player.UID);

            PopulateWorld();

            //Sends the player cube and starting food cubes to the client
            lock (world)
            {
                SendInitialData(player);
            }
            state.sb.Clear();
            Network.i_want_more_data(state);
        }

        private static void SendInitialData(Cube player)
        {
            lock (world)
            {
                Network.Send(dataSocket, JsonConvert.SerializeObject(player) + "\n");
                StringBuilder builder = new StringBuilder();
                foreach (Cube cube in world.food_cubes.Values)
                {
                    builder.Append(JsonConvert.SerializeObject(cube) + "\n");
                }
                Network.Send(dataSocket, builder.ToString());
            }
        }
        
        //Handle data from the client
        static void HandleData(Preserved_State state)
        {
            string movement = state.sb.ToString();
            int player_uid = state.UID;

            string type;
            int x, y;
            if (TryParseData(movement, out type, out x, out y))
            {
                playerMovementQueue.Enqueue(new Tuple<string, int, int, int>(type, x, y, player_uid));
            }
            state.sb.Clear();
            System.Threading.Thread.Sleep(1000 / world.HEARTBEATS_PER_SECOND);
            Network.i_want_more_data(state);
        }

        /// <summary>
        /// Puts the data into a usable format, and we only use the
        /// first request in the queue.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static bool TryParseData(string data, out string type, out int x, out int y)
        {
            string[] parts = data.Split('\n');
            string temp_type = Regex.Replace(parts[0], "[()]", "");
            parts = temp_type.Split(',');
            type = "";
            x = y = 0;

            try {
                //Check to see if the request was either for move or split
                if (int.TryParse(parts[1], out x) && int.TryParse(parts[2], out y))
                {
                    if (parts[0].Equals("move"))
                    {
                        type = parts[0];
                        return true;
                    }
                    else if (parts[0].Equals("split"))
                    {
                        type = parts[1];
                        return true;
                    }
                }
            }
            catch(Exception e)
            {
                return false;
            }
            
            return false;
        }
        /********************************* END HANDLE NETWORK COMMUNICATIONS ********************/




        /************************************ HANDLE GAMEPLAY MECHANICS *************************/
        private static void PopulateWorld()
        {
            for (int i = 0; i < world.MAX_FOOD / 2; i++)
            {
                world.AddFoodCube();
            }
        }
        /********************************** END HANDLE GAMEPLAY MECHANICS ***********************/
    }
}
