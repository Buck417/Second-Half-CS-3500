using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Network_Controller;

namespace Server
{
    class AgServer
    {
        World world;

        static void Main(string[] args)
        {
            AgServer server = new AgServer();
            server.world = new World();
            Network.Server_Awaiting_Client_Loop(new AsyncCallback(Handle_New_Client_Connections));
        }

        //Start


        //Handle new client connections
        static void Handle_New_Client_Connections(IAsyncResult ar)
        {
            Preserved_State state = (Preserved_State)ar.AsyncState;
            state.callback = Receive_Player_Name;
        }

        //Receive the player name
        static void Receive_Player_Name(IAsyncResult ar)
        {

        }

        //Handle data from the client


        //Update


    }
}
