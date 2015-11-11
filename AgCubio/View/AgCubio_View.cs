using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Model;

namespace View
{
    public partial class AgCubio_View : Form
    {
        private System.Drawing.SolidBrush myBrush;

        World world;


        public AgCubio_View()
        {
            InitializeComponent();

            world = new World();

            //Use this to prevent screen flickering when redrawing the world
            DoubleBuffered = true;
        }

        int count = 0;
        public void AgCubioPaint(object sender, PaintEventArgs e)
        {
            Color color = Color.FromArgb(count, count, count);
            myBrush = new System.Drawing.SolidBrush(color);

            count++;

            if (count > 255) count = 0;

            e.Graphics.FillRectangle(myBrush, new Rectangle(count, count, 10 + count, 10 + 2 * count));
            Console.WriteLine("repainting " + count);
            this.Invalidate();
        }


        /***************************************CALLBACK DELEGATES*****************************************/

        public void ProcessIncomingCubeJson(IAsyncResult state)
        {
            Byte[] bytes = (Byte[])state.AsyncState;
            String json = Encoding.UTF8.GetString(bytes);
            Cube c = Cube.Create(json);
            lock(world){                                           
                world.ProcessIncomingCube(c);
            }
            ///TODO: Draw method invoke
            ///

        }




        /***************************************CALLBACK DELEGATES*****************************************/


    }
}
