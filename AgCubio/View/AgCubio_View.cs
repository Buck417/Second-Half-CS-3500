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
        public string PlayerName, GameHost;
        
        public AgCubio_View()
        {
            InitializeComponent();

            Form1 start_game_popup = new Form1(this);
            start_game_popup.ShowDialog(this);
            
            start_game_popup.FormClosed += play_button_click;

            //Use this to prevent screen flickering when redrawing the world
            DoubleBuffered = true;

            world = new World();
        }
        
        public void AgCubioPaint(object sender, PaintEventArgs e)
        {
            //Compute the x and y offset, based on where the player cube is and how big it is.
            int center_x = this.Width / 2;
            int center_y = this.Height / 2;
            Cube player_cube = world.GetCube(world.Player_UID);
            world.xoff = (player_cube.X + (player_cube.Width / 2)) - center_x;
            world.yoff = (player_cube.Y + (player_cube.Width / 2)) - center_y;
            
            Invalidate();
        }

        /***************************************CALLBACK DELEGATES*****************************************/

        private void ConnectCallback(IAsyncResult ar)
        {
            Preserved_State state = (Preserved_State)ar.AsyncState;
            state.GUI_Callback = new AsyncCallback(ReceivePlayer);
            
            //Send the player name
            Network_Controller.Network_Controller.Send(socket, PlayerName);
        }

        private void ReceivePlayer(IAsyncResult ar)
        {
            Preserved_State state = (Preserved_State)ar.AsyncState;
            Console.WriteLine("Welcome " + PlayerName);
            state.GUI_Callback = new AsyncCallback(ReceiveData);
            ReceiveData(ar);
        }

        private void ReceiveData(IAsyncResult ar)
        {
            //Get them cubes

        }
        /***************************************CALLBACK DELEGATES*****************************************/




        /********************************************* HELPER METHODS *********************************************/
        private void ProcessJsonBlock(string block)
        {
            foreach (string line in block.Split('\n'))
            {
                ProcessJsonLine(line);
            }
        }

        private void ProcessJsonLine(string json)
        {
            Cube cube = Cube.Create(json);
            lock (world)
            {
                world.ProcessIncomingCube(cube);
            }

            lock (world)
            {
                //DrawCube(cube);
            }

        }
        
        private void DrawWorld(Cube cube, PaintEventArgs e)
        {
            Color color = Color.FromArgb(cube.Color);
            myBrush = new System.Drawing.SolidBrush(color);

            e.Graphics.FillRectangle(myBrush, new Rectangle(cube.X - world.xoff, cube.Y - world.yoff, cube.Width * world.Scale, cube.Width * world.Scale));
        }
        
        private void SendMoveRequest(int x, int y)
        {
            string data = "(move, " + x + ", " + y + ")\n";
            Socket socket = null;
            Network_Controller.Network_Controller.Send(socket, data);
        }

        private void SendSplitRequest(int x, int y)
        {
            string data = "(split, " + x + ", " + y + ")\n";
            Socket socket = null;
            Network_Controller.Network_Controller.Send(socket, data);
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
            int mouse_x = e.X - world.xoff;
            int mouse_y = e.Y - world.yoff;

            SendMoveRequest(mouse_x, mouse_y);
        }

        private void play_button_click(object sender, EventArgs e)
        {
            Network_Controller.Network_Controller.Connect_To_Server(new AsyncCallback(ConnectCallback), GameHost);
        }
        /******************************************* END LISTENERS ***********************************************/
    }
    
}
