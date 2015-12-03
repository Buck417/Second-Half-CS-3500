using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Network_Controller
{

    public class Preserved_State
    {
        public Action<Preserved_State> callback;

        public Socket socket = null;
        public byte[] buffer = new byte[Network.BUFFER_SIZE];
        public StringBuilder sb = new StringBuilder();
        public int UID = 0;
    }

    public static class Network
    {
        private static UTF8Encoding encoding = new UTF8Encoding();
        public static int BUFFER_SIZE = 1024;

        /// <summary>
        /// This guy connects to the server.
        /// This function should attempt to connect to the server via a provided hostname. It should save the callback function (in a state object) for use when data arrives.
        /// It will need to open a socket and then use the BeginConnect method. Note this method takes the "state" object and "regurgitates" it back to you when a connection is made, thus allowing "communication" between this function and the Connected_to_Server function.
        /// </summary>
        /// <param name="callBack">A function inside the View to be called when a connection is made</param>
        /// <param name="hostName">The name of the server to connect to</param>
        /// <returns></returns>
        public static Socket Connect_To_Server(Action<Preserved_State> callBack, string hostName)
        {
            try {
                Socket new_socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

                Preserved_State state = new Preserved_State
                {
                    callback = callBack,
                    socket = new_socket,
                };


                new_socket.BeginConnect(hostName, 11000, new AsyncCallback(Connected_to_Server), state);

                return new_socket;
            }
            catch(Exception e)
            {

            }

            return null;
        }

        /// <summary>
        /// This function is referenced by the BeginConnect method above and is "called" by the OS when the socket connects to the server. The "state_in_an_ar_object" object contains a field "AsyncState" which contains the "state" object saved away in the above function.
        /// Once a connection is established the "saved away" callback function needs to called. Additionally, the network connection should "BeginReceive" expecting more data to arrive(and provide the ReceiveCallback function for this purpose)
        /// </summary>
        /// <param name="state_in_an_ar_object"></param>
        static void Connected_to_Server(IAsyncResult ar)
        {
            Preserved_State state = (Preserved_State)ar.AsyncState;
            
            state.socket.EndConnect(ar);

            //Call the first callback, which resets some info in the state
            state.callback(state);

            state.buffer = new byte[BUFFER_SIZE];
            if (state.socket.Connected)
            {
                state.socket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
            }
        }

        /// <summary>
        /// If anything happens with the socket, close gracefully
        /// </summary>
        /// <param name="socket"></param>
        private static void CloseGracefully(Socket socket)
        {
            Console.WriteLine("Connection lost");
            socket.Close();
            return;
        }

        /// <summary>
        /// The ReceiveCallback method is called by the OS when new data arrives. This method should check to see how much data has arrived. If 0, the connection has been closed (presumably by the server). On greater than zero data, this method should call the callback function provided above.
        /// For our purposes, this function should not request more data.It is up to the code in the callback function above to request more data.
        /// </summary>
        /// <param name="state_in_an_ar_object"></param>
        static void ReceiveCallback(IAsyncResult ar)
        {
            Preserved_State state = (Preserved_State)ar.AsyncState;
            Socket socket = state.socket;

            byte[] buffer = (byte[])state.buffer;

            //If the connection was forcibly closed on the server's end, handle that
            if(socket.Connected == false)
            {
                CloseGracefully(socket);
                return;
            }

            //Try to stop receiving this current request
            int byte_count = 0;
            try {
                byte_count = socket.EndReceive(ar);
            }
            catch(Exception e)
            {
                //If we couldn't stop the reception, close gracefully
                CloseGracefully(socket);
            }


            if (byte_count == 0)
            {
                //Connection lost
                CloseGracefully(socket);
                return;
            }
            else
            {
                string the_string = encoding.GetString(buffer, 0, byte_count);
                state.sb.Append(the_string);

                //If the last character is a newline, we're done receiving, so we can call the callback in the GUI
                if (the_string.LastIsNewline())
                {
                    state.callback(state);
                }
                //If the last character isn't a newline, we know we're not done receiving yet, so we need to ask for more data (after appending to the string builder
                else
                {
                    i_want_more_data(state);
                }
            }
        }

        /// <summary>
        /// This is a small helper function that the client View code will call whenever it wants more data. Note: the client will probably want more data every time it gets data.
        /// </summary>
        /// <param name="state"></param>
        public static void i_want_more_data(Preserved_State state)
        {
            
            state.socket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
        }

        /// <summary>
        /// This function (along with its helper 'SendCallback') will allow a program to send data over a socket. This function needs to convert the data into bytes and then send them using socket.BeginSend.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        public static void Send(Socket socket, String data)
        {
            if (socket == null || !socket.Connected) return;

            //Convert the string into a byte array
            byte[] byte_data = Encoding.UTF8.GetBytes(data);
            byte[] buffer = new byte[BUFFER_SIZE];
            try {
                socket.BeginSend(byte_data, 0, byte_data.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// This function "assists" the Send function. If all the data has been sent, then life is good and nothing needs to be done (note: you may, when first prototyping your program, put a WriteLine in here to see when data goes out).
        /// If there is more data to send, the SendCallBack needs to arrange to send this data(see the ChatClient example program).
        /// </summary>
        private static void SendCallBack(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;


            //Find out how many bytes were sent
            try {
                int bytes = socket.EndSend(ar);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// This is the heart of the server code. It should ask the OS to listen for a connection and save the callback function with that request. 
        /// Upon a connection request coming in the OS should invoke the Accept_a_New_Client method (see below).
        /// 
        /// Note: while this method is called "Loop", it is not a traditional loop, but an "event loop" (i.e., this method sets up the connection listener, 
        /// which, when a connection occurs, sets up a new connection listener. for another connection).
        /// </summary>
        /// <param name="callback"></param>
        public static void Server_Awaiting_Client_Loop(Action<Preserved_State> callback)
        {   
            //Create socket that can accept both IPv4 and IPv6
            Socket server_socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            server_socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

            //Try having the socket listen to the port for any IPaddress and accept
            try
            {
                server_socket.Bind(new IPEndPoint(IPAddress.IPv6Any, 11000));
                server_socket.Listen(100);


                Preserved_State state = new Preserved_State
                {
                    callback = callback,
                    socket = server_socket,
                };
                
                Console.WriteLine("Waiting for a connection...\n");

                server_socket.BeginAccept(new AsyncCallback(Accept_A_New_Client),state);
                Console.Read();     //Keeps the thread open because it doesn't connect automatically

            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// This code should be invoked by the OS when a connection request comes in. It should:
        /// 1. Create a new socket
        /// 2. Call the callback provided by the above method
        /// 3. Await a new connection request.
        /// 
        /// Note: the callback method referenced in the above function should have been transferred to this 
        /// function via the AsyncResult parameter and should be invoked at this point.
        /// </summary>
        /// <param name="ar"></param>
        public static void Accept_A_New_Client(IAsyncResult ar)
        {
            Console.Write("A new client has connected\n");
            Preserved_State state = (Preserved_State)ar.AsyncState;

            Socket listener = state.socket;
            Socket handler = listener.EndAccept(ar);

            Preserved_State state2 = new Preserved_State
            {
                socket = handler,

            };

            state.callback(state2);


            
            listener.BeginAccept(Accept_A_New_Client, state);
        }




        /********************************************* HELPER METHODS *********************************************/

        /// <summary>
        /// Helper method to see if the last char is a newline (for our protocol, that's how we know it's finished)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool LastIsNewline(this string str)
        {
            return (str.LastIndexOf('\n')) == (str.Length - 1);
        }

        /********************************************* END HELPER METHODS *****************************************/
    }
}
