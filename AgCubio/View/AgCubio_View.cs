using System;
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
        public string PlayerName = "Ryan", GameHost = "127.0.0.1";
        private bool GameStarted = false;
        
        public AgCubio_View()
        {
            InitializeComponent();
            //Use this to prevent screen flickering when redrawing the world
            DoubleBuffered = true;

            /*
            Form1 start_game_popup = new Form1(this);
            start_game_popup.ShowDialog(this);
            start_game_popup.FormClosed += play_button_click;
            */

            StartGame();

            world = new World();
        }
        
        public void AgCubioPaint(object sender, PaintEventArgs e)
        {
            //Only run this if the game has started
            if (!GameStarted) return;
            
            //Compute the x and y offset, based on where the player cube is and how big it is.
            int center_x = this.Width / 2;
            int center_y = this.Height / 2;
            Cube player_cube = world.GetCube(world.Player_UID);
            world.xoff = (player_cube.X + (player_cube.Width / 2)) - center_x;
            world.yoff = (player_cube.Y + (player_cube.Width / 2)) - center_y;
            
            foreach(Cube cube in world.cubes.Values)
            {
                DrawCube(cube, e);
            }

            Invalidate();
        }

        /***************************************CALLBACK DELEGATES*****************************************/

        private void ConnectCallback(IAsyncResult ar)
        {
            Preserved_State state = (Preserved_State)ar.AsyncState;
            state.GUI_Callback = new AsyncCallback(ReceivePlayer);
            
            //Send the player name
            Network.Send(socket, PlayerName + '\n');
        }
        
        private void ReceivePlayer(IAsyncResult ar)
        {
            Preserved_State state = (Preserved_State)ar.AsyncState;
            Console.WriteLine("Welcome " + PlayerName);

            //Handle the player cube coming in
            string player_json = state.sb.ToString();
            ProcessJsonBlock(player_json);
            
            state.GUI_Callback = new AsyncCallback(ReceiveData);
            Network.i_want_more_data(ar);
        }

        private void ReceiveData(IAsyncResult ar)
        {
            //Get them cubes
            Preserved_State state = (Preserved_State)ar.AsyncState;
            string cube_json = state.sb.ToString();
            ProcessJsonBlock(cube_json);
            
            Network.i_want_more_data(ar);
        }
        /***************************************CALLBACK DELEGATES*****************************************/




        /********************************************* HELPER METHODS *********************************************/
        private void ProcessJsonBlock(string block)
        {
            foreach (string line in block.Split('\n'))
            {

                if (line.Equals("")) continue;

                //Once we get to \0, we know it's an empty byte, so we don't need to keep looking for more data.
                else if (line.Contains("\0")) break;

                //See if the cube was created with valid JSON. If it wasn't created, it's because the JSON was invalid from being only part of the string.
                Cube cube = Cube.Create(line);
                if (cube == null) break;

                ProcessJsonCube(cube);
            }
        }

        private void ProcessJsonCube(Cube cube)
        {
            lock (world)
            {
                world.ProcessIncomingCube(cube);
            }

            lock (world)
            {
                Invalidate();
            }
        }
        
        private void DrawCube(Cube cube, PaintEventArgs e)
        {
            Color color = Color.FromArgb(cube.Color);
            myBrush = new System.Drawing.SolidBrush(color);

            e.Graphics.FillRectangle(myBrush, new Rectangle(cube.X - world.xoff, cube.Y - world.yoff, cube.Width * world.Scale, cube.Width * world.Scale));
        }
        
        private void SendMoveRequest(int x, int y)
        {
            string data = "(move, " + x + ", " + y + ")\n";
            Socket socket = null;
            Network.Send(socket, data);
        }

        private void SendSplitRequest(int x, int y)
        {
            string data = "(split, " + x + ", " + y + ")\n";
            Socket socket = null;
            Network.Send(socket, data);
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
            StartGame();
        }

        private void StartGame()
        {
            //Start by connecting to the server
            socket = Network.Connect_To_Server(new AsyncCallback(ConnectCallback), GameHost);
        }
        /******************************************* END LISTENERS ***********************************************/
    }
    
}
