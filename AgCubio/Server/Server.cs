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

        public static void Main(string[] args)
        {
            AgServer server = new AgServer();
            world = new World();
            Network.Server_Awaiting_Client_Loop(new Action<Preserved_State>(Handle_New_Client_Connections));
        }

        private static void HeartbeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (world)
            {
                world.AddFoodCube();
                world.Update();
                SendWorld();
            }
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

            PopulateWorld();

            //Sends the player cube and starting food cubes to the client
            lock (world)
            {
                SendPlayer(player);
                SendWorld();
            }
            Network.i_want_more_data(state);
        }

        private static void SendPlayer(Cube player)
        {
            Network.Send(dataSocket, JsonConvert.SerializeObject(player) + "\n");
        }

        private static void SendWorld()
        {
            StringBuilder string_builder = new StringBuilder();
            lock (world)
            {
                foreach (Cube cube in world.cubes.Values)
                {
                    string_builder.Append(JsonConvert.SerializeObject(cube) + "\n");
                }
                Network.Send(dataSocket, string_builder.ToString());
            }
        }

        //Handle data from the client
        static void HandleData(Preserved_State state)
        {
            //Preserved_State state = (Preserved_State)ar.AsyncState;
            string str = state.sb.ToString();

            world.ProcessData(str);

            Network.i_want_more_data(state);
        }
        /********************************* END HANDLE NETWORK COMMUNICATIONS ********************/




        /************************************ HANDLE GAMEPLAY MECHANICS *************************/
        private static void PopulateWorld()
        {
            for(int i = 0; i < world.MAX_FOOD; i++)
            {
                world.AddFoodCube();
            }
            for(int i = 0; i < world.VIRUS_COUNT; i++)
            {
                world.AddVirusCube();
            }
        }



        /********************************** END HANDLE GAMEPLAY MECHANICS ***********************/
    }
}
