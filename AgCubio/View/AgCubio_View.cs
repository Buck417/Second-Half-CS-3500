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

        public AgCubio_View()
        {
            InitializeComponent();

            //Use this to prevent screen flickering when redrawing the world
            DoubleBuffered = true;

            world = new World();
        }

        public void AgCubioPaint(object sender, PaintEventArgs e)
        {
            Cube json_object = new Cube(926, 682, -65536, 5571, false, "name", 1000);
            String json_string = JsonConvert.SerializeObject(json_object);
            Cube cube = JsonConvert.DeserializeObject<Cube>(json_string);

            //Compute the x and y offset, based on where the player cube is and how big it is.
            int center_x = this.Width / 2;
            int center_y = this.Height / 2;
            Cube player_cube = world.GetCube(world.Player_UID);
            world.xoff = (player_cube.X + (player_cube.Width / 2)) - center_x;
            world.yoff = (player_cube.Y + (player_cube.Width / 2)) - center_y;

            DrawCube(cube, e);
            
            Invalidate();
        }

        /***************************************CALLBACK DELEGATES*****************************************/

        public void ProcessIncomingCubeJson(IAsyncResult state)
        {
            Byte[] bytes = (Byte[])state.AsyncState;
            String json = Encoding.UTF8.GetString(bytes);
            ProcessJsonBlock(json);
        }

        public void ProcessIncomingLogin(IAsyncResult state)
        {

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
        /******************************************* END LISTENERS ***********************************************/
    }
}
