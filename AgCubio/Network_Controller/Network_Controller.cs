using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Network_Controller
{

    public class Preserved_State
    {

        Delegate ReceiveCallback;
    }

    public static class Network_Controller
    {
        

        /// <summary>
        /// This guy connects to the server.
        /// This function should attempt to connect to the server via a provided hostname. It should save the callback function (in a state object) for use when data arrives.
        ///It will need to open a socket and then use the BeginConnect method. Note this method takes the "state" object and "regurgitates" it back to you when a connection is made, thus allowing "communication" between this function and the Connected_to_Server function.
        /// </summary>
        /// <param name="callBack">A function inside the View to be called when a connection is made</param>
        /// <param name="hostName">The name of the server to connect to</param>
        /// <returns></returns>
        public static Socket Connect_To_Server(Delegate callBack, string hostName)
        {
            
            return null;
        }

        /// <summary>
        /// This function is referenced by the BeginConnect method above and is "called" by the OS when the socket connects to the server. The "state_in_an_ar_object" object contains a field "AsyncState" which contains the "state" object saved away in the above function.
        /// Once a connection is established the "saved away" callback function needs to called. Additionally, the network connection should "BeginReceive" expecting more data to arrive(and provide the ReceiveCallback function for this purpose)
        /// </summary>
        /// <param name="state_in_an_ar_object"></param>
        static void Connected_to_Server(IAsyncResult ar_state)
        {

        }

        /// <summary>
        /// The ReceiveCallback method is called by the OS when new data arrives. This method should check to see how much data has arrived. If 0, the connection has been closed (presumably by the server). On greater than zero data, this method should call the callback function provided above.
        /// For our purposes, this function should not request more data.It is up to the code in the callback function above to request more data.
        /// </summary>
        /// <param name="state_in_an_ar_object"></param>
        static void ReceiveCallback(IAsyncResult ar_state)
        {

        }

        /// <summary>
        /// This is a small helper function that the client View code will call whenever it wants more data. Note: the client will probably want more data every time it gets data.
        /// </summary>
        /// <param name="state"></param>
        static void i_want_more_data(IAsyncResult state)
        {

        }

        /// <summary>
        /// This function (along with it's helper 'SendCallback') will allow a program to send data over a socket. This function needs to convert the data into bytes and then send them using socket.BeginSend.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        static void Send(Socket socket, String data)
        {

        }

        /// <summary>
        /// This function "assists" the Send function. If all the data has been sent, then life is good and nothing needs to be done (note: you may, when first prototyping your program, put a WriteLine in here to see when data goes out).
        ///If there is more data to send, the SendCallBack needs to arrange to send this data(see the ChatClient example program).
        /// </summary>
        static void SendCallBack()
        {

        }
    }
}
