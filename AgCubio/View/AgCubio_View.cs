﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Model;
using Network_Controller;
using Newtonsoft.Json;
using System.Threading;
using System.Net.Sockets;

namespace View
{
    public partial class AgCubio_View : Form
    {
        private System.Drawing.SolidBrush myBrush;
        private World world;
        private Socket socket;
        public string player_name = "Nobody", GameHost = "localhost";
        private bool GameRunning = false, ServerConnected = false;
        private int dest_x, dest_y;
        private int player_mass;

        /// <summary>
        /// Upon starting the game, show the initial startup screen
        /// and initialize the world.
        /// </summary>
        public AgCubio_View()
        {
            InitializeComponent();
            //Use this to prevent screen flickering when redrawing the world
            DoubleBuffered = true;

            Form1 start_game_popup = new Form1(this, false);
            start_game_popup.ShowDialog(this);

            world = new World();
        }

        /// <summary>
        /// This is kind of like our event loop, where everything is drawn.
        /// The world is the key to lock on, where processing cubes and 
        /// drawing them are done in locked fashion so as to not change
        /// the underlying data unecessarily.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AgCubioPaint(object sender, PaintEventArgs e)
        {
            //If the game hasn't started yet, show the inital setup form
            if (!GameRunning)
            {
                return;
            }
            else if (GameRunning)
            {
                if (ServerConnected)
                {
                    try
                    {
                        //Compute the x and y offset, based on where the player cube is and how big it is.
                        int center_x = this.Width / 2;
                        int center_y = this.Height / 2;
                        Cube player_cube = world.GetPlayerCube();

                        //If the player cube isn't in the world anymore, we know it's game over
                        if (player_cube == null)
                        {
                            Console.WriteLine("Game over!");
                            GameRunning = false;
                            GameOverForm game_over = new GameOverForm(this, player_mass, player_name);
                            game_over.ShowDialog(this);

                            return;
                        }
                        player_mass = (int)world.GetPlayerMass();
                        //world.Scale = (this.Width / (player_cube.Width * 10));

                        lock (world)
                        {

                            //Draw the player cube first
                            DrawCube(player_cube, e);

                            foreach (Cube cube in world.cubes.Values)
                            {
                                if (cube == player_cube) continue;
                                DrawCube(cube, e);
                            }

                            System.Drawing.Font drawFont = new System.Drawing.Font("Arial", (int)(10 * world.Scale));
                            System.Drawing.SolidBrush nameBrush = new System.Drawing.SolidBrush(Color.FromName("black"));

                            e.Graphics.DrawString("Frames per second: " + CalculateFrameRate(), drawFont, nameBrush, new PointF(this.Width - 300, 50));
                            e.Graphics.DrawString("Player mass: " + (int)player_mass, drawFont, nameBrush, new PointF(this.Width - 300, 75));
                        }
                        
                        //Check to see if the player cube is where we told it to go. If not, send a move request again.
                        if (player_cube.X != dest_x || player_cube.Y != dest_y)
                        {
                            SendMoveRequest(dest_x, dest_y);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    Invalidate();
                }
            }
        }

        /***************************************CALLBACK DELEGATES*****************************************/
        /// <summary>
        /// Callback for the connection 
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            Preserved_State state = (Preserved_State)ar.AsyncState;

            state.GUI_Callback = new AsyncCallback(ReceivePlayer);

            if (state.socket.Connected)
            {
                //Send the player name
                ServerConnected = true;
                Network.Send(socket, player_name + '\n');
            }
            else
            {
                ServerConnected = false;
                Invoke(new Action(ShowReConnectForm));
            }
        }

        /// <summary>
        /// Receive the initial player response
        /// </summary>
        /// <param name="ar"></param>
        private void ReceivePlayer(IAsyncResult ar)
        {
            Preserved_State state = (Preserved_State)ar.AsyncState;
            Console.WriteLine("Welcome " + player_name);

            //Handle the player cube coming in
            string player_json = state.sb.ToString();
            world.AddPlayerCube(player_json);
            GameRunning = true;

            state.GUI_Callback = new AsyncCallback(ReceiveData);
            Network.i_want_more_data(ar);
        }

        /// <summary>
        /// Callback to be sent to the network to receive more data
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveData(IAsyncResult ar)
        {
            //Get them cubes
            Preserved_State state = (Preserved_State)ar.AsyncState;
            string world_json = state.sb.ToString();
            ProcessJsonBlock(world_json);

            //Clear out what was in the last version of the world so it gets a fresh copy
            state.sb.Clear();

            Network.i_want_more_data(ar);
        }
        /***************************************CALLBACK DELEGATES*****************************************/




        /********************************************* HELPER METHODS *********************************************/
        /// <summary>
        /// This takes care of an entire block of JSON, i.e. the world
        /// </summary>
        /// <param name="block"></param>
        private void ProcessJsonBlock(string block)
        {
            string[] json_blocks = block.Split('\n');

            //Check the last item to see if it was an entire json block. If not, skip over it.
            if (!json_blocks[json_blocks.Length - 1].Contains('\n'))
            {
                json_blocks[json_blocks.Length - 1] = "";
            }

            foreach (string json in json_blocks)
            {
                if (json.Equals("")) continue;
                Cube cube = Cube.Create(json);
                ProcessJsonCube(cube);
            }
        }

        /// <summary>
        /// Abstracts the processing of the cube
        /// </summary>
        /// <param name="cube"></param>
        private void ProcessJsonCube(Cube cube)
        {
            lock (world)
            {
                world.ProcessIncomingCube(cube);
            }
            
            Invalidate();
        }

        /// <summary>
        /// This does the nuts and bolts for drawing the cube
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="e"></param>
        private void DrawCube(Cube cube, PaintEventArgs e)
        {
            Color color = Color.FromArgb(cube.Color);
            myBrush = new System.Drawing.SolidBrush(color);

            e.Graphics.FillRectangle(myBrush, new Rectangle((int)(cube.X - (cube.Width * world.Scale / 2)), (int)(cube.Y - (cube.Width * world.Scale / 2)), (int)(cube.Width * world.Scale), (int)(cube.Width * world.Scale)));

            System.Drawing.Font drawFont = new System.Drawing.Font("Quartz MS", (int)(10 * world.Scale));
            System.Drawing.SolidBrush nameBrush = new System.Drawing.SolidBrush(Color.FromName("yellow"));
            StringFormat string_format = new StringFormat();
            string_format.Alignment = StringAlignment.Center;
            string_format.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString(cube.Name, drawFont, nameBrush, new PointF(cube.GetCenterX() - (cube.Width / 2), cube.GetCenterY() - (cube.Width / 2)), string_format);

            //e.Graphics.ScaleTransform(1.5f, 1.5f);
        }

        /// <summary>
        /// Typically this is first called when the mouse moves. It's a helper method
        /// for the mouse listener to send a move request to the server.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SendMoveRequest(int x, int y)
        {
            //If the player cube isn't at the place we're sending it to yet, keep sending the move request
            string data = "(move, " + x + ", " + y + ")\n";
            Network.Send(socket, data);
        }

        /// <summary>
        /// This is a helper method for the spacebar key listener to send a split request
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SendSplitRequest(int x, int y)
        {
            string data = "(split, " + x + ", " + y + ")\n";
            Network.Send(socket, data);
        }

        private static int lastTick;
        private static int lastFrameRate;
        private static int frameRate;

        /// <summary>
        /// This guy calculates the frame rate for our game
        /// </summary>
        /// <returns></returns>
        private static int CalculateFrameRate()
        {

            if (System.Environment.TickCount - lastTick >= 1000)
            {
                lastFrameRate = frameRate;
                frameRate = 0;
                lastTick = System.Environment.TickCount;
            }
            frameRate++;
            return lastFrameRate;
        }

        /// <summary>
        /// Whenever a connection drops, this form shows up, and you get
        /// to try connecting again.
        /// </summary>
        private void ShowReConnectForm()
        {
            Form1 form = new Form1(this, true);
            form.ShowDialog(this);
        }
        /******************************************* END HELPER METHODS ******************************************/



        /********************************************* LISTENERS *************************************************/
        private void AgCubio_View_KeyPress(object sender, KeyPressEventArgs e)
        {
            //If the user presses spacebar (32), send a split request based on the mouse's current position
            if (e.KeyChar == 32)
            {
                int dest_x = Cursor.Position.X;
                int dest_y = Cursor.Position.Y;
                SendSplitRequest(dest_x, dest_y);
            }
        }

        private void AgCubio_View_MouseMove(object sender, MouseEventArgs e)
        {
            int mouse_x = dest_x = e.X;// - world.xoff;
            int mouse_y = dest_y = e.Y;// - world.yoff;

            SendMoveRequest(mouse_x, mouse_y);
        }

        /// <summary>
        /// This is a helper method to "start the game", i.e. to connect to 
        /// the server and start receiving data.
        /// </summary>
        public void StartGame()
        {
            //Start by connecting to the server
            socket = Network.Connect_To_Server(new AsyncCallback(ConnectCallback), GameHost);
        }
        /******************************************* END LISTENERS ***********************************************/
    }

}
