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
    public partial class Start_Game_Form : Form
    {
        public string PlayerName, Host;
        private AgCubio_View view;

        public Start_Game_Form(AgCubio_View view, bool restart)
        {
            InitializeComponent();
            view.GameHost = game_host_box.Text;
            if (restart) connection_error_label.Visible = true;
            else connection_error_label.Visible = false;
            this.view = view;
        }

        private void name_box_TextChanged(object sender, EventArgs e)
        {
            view.player_name = name_box.Text;
        }

        private void game_host_box_TextChanged(object sender, EventArgs e)
        {
            view.GameHost = game_host_box.Text;
        }

        private void inputKeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)13)
            {
                view.StartGame();
                this.Close();
            }
        }

        private void play_button_Click(object sender, EventArgs e)
        {
            view.StartGame();
            this.Close();
        }

        
    }
}
