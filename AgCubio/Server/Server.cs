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
        
        public static int player_uid;

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
            lock (world)
            {

                Network.Send(dataSocket, JsonConvert.SerializeObject(world.AddFoodCube()) + "\n");
          
                foreach (Cube cube in cubes_eaten)
                {
                    Network.Send(dataSocket, JsonConvert.SerializeObject(cube) + "\n");
                }

                foreach (Cube cube in world.player_cubes.Values)
                {
                    Network.Send(dataSocket, JsonConvert.SerializeObject(cube) + "\n");
                }

                


                //world.Update();
            }
            
            //SendWorld();

            heartbeatTimer.Start();


        }

        /*********************************** HANDLE NETWORK COMMUNICATIONS **********************/
        //Handle new client connections
        private static void Handle_New_Client_Connections(Preserved_State state)
        {
            dataSocket = state.socket;
            state.callback = new Action<Preserved_State>(Receive_Player_Name);
            Network.i_want_more_data(state);

            heartbeatTimer.Interval = 1000 / world.HEARTBEATS_PER_SECOND;
            heartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
            heartbeatTimer.Start();
        }

        //Receive the player name
        private static void Receive_Player_Name(Preserved_State state)
        {
            //Preserved_State state = (Preserved_State)ar.AsyncState;
            state.callback = new Action<Preserved_State>(HandleData);
            string playerName = state.sb.ToString();


            Cube player = world.AddPlayerCube(playerName);
            player_uid = player.UID;

            PopulateWorld();

            //Sends the player cube and starting food cubes to the client
            lock(world)
            {
                SendInitialData(player);
            }
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

        //private static void SendPlayer(Cube player)
        //{

        //    Network.Send(dataSocket, JsonConvert.SerializeObject(player) + "\n");

        //}

        //private static void SendWorld(Preserved_State state)
        //{

        //    StringBuilder string_builder = new StringBuilder();

        //    foreach (Cube cube in world.cubes.Values)
        //    {
        //        Network.Send(dataSocket, JsonConvert.SerializeObject(cube) + "\n");
        //    }


        //}

        //Handle data from the client
        static void HandleData(Preserved_State state)
        {
            //Preserved_State state = (Preserved_State)ar.AsyncState;
            string str = state.sb.ToString();

            lock (world)
            {
                world.ProcessData(str, player_uid);
            }
            //SendWorld(state);
            //heartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;



            state.sb.Clear();
            System.Threading.Thread.Sleep(1000/world.HEARTBEATS_PER_SECOND);
            Network.i_want_more_data(state);
        }
        /********************************* END HANDLE NETWORK COMMUNICATIONS ********************/




        /************************************ HANDLE GAMEPLAY MECHANICS *************************/
        private static void PopulateWorld()
        {
            for (int i = 0; i < world.MAX_FOOD / 2; i++)
            {
                world.AddFoodCube();
            }
            //for (int i = 0; i < world.VIRUS_COUNT; i++)
            //{
            //    world.AddVirusCube();
            //}

        }



        /********************************** END HANDLE GAMEPLAY MECHANICS ***********************/
    }
}
