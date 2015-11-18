using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace View
{
    public partial class GameOverForm : Form
    {
        private AgCubio_View master_view;

        public GameOverForm(AgCubio_View view, int mass, string player_name)
        {
            InitializeComponent();
            name_label.Text = "Name: " + player_name;
            mass_label.Text = "Mass: " + mass.ToString();
            master_view = view;
        }

        private void restart_click_handler(object sender, EventArgs e)
        {
            this.Close();
            master_view.StartGame();
        }
    }
}
