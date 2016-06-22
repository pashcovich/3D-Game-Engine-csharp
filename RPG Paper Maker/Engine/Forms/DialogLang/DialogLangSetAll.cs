﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPG_Paper_Maker
{
    public partial class DialogLangSetAll : Form
    {
        public string Content = "";

        public DialogLangSetAll(string txt)
        {
            InitializeComponent();
            textBox1.Text = txt;
        }

        private void ok_Click(object sender, EventArgs e)
        {
            Content = textBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
