﻿using System;
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
    public partial class Form1 : Form
    {
        public string PlayerName, Host;
        private AgCubio_View view;

        public Form1(AgCubio_View view)
        {
            InitializeComponent();
            this.view = view;
        }

        private void name_box_TextChanged(object sender, EventArgs e)
        {
            view.PlayerName = name_box.Text;
        }

        private void game_host_box_TextChanged(object sender, EventArgs e)
        {
            view.GameHost = game_host_box.Text;
        }

        private void play_button_Click(object sender, EventArgs e)
        {
            view.StartGame();
            this.Close();
        }

        
    }
}
