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

namespace View
{
    public partial class AgCubio_View : Form
    {
        private System.Drawing.SolidBrush myBrush;
        private World world;

        public AgCubio_View()
        {
            InitializeComponent();

            //Use this to prevent screen flickering when redrawing the world
            DoubleBuffered = true;

            world = new World();
        }

        /// <summary>
        /// Send the name to the specified host
        /// </summary>
        public void DoLogin(string name, string host)
        {
            

        }

        

        public void ProcessCube()
        {

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


        /********************************************* HELPER METHODS *********************************************/
        private void ProcessJsonBlock(string block)
        {
            foreach(string line in block.Split('\n'))
            {
                ProcessJsonLine(line);
            }
        }

        private void ProcessJsonLine(string line)
        {
            Cube c = Cube.Create(line);
        }

        private void DrawCube(Cube cube)
        {

        }
        /******************************************* END HELPER METHODS ******************************************/
    }
}
