﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GerenciarSenhas.Interface
{
    public partial class frmOutros : Form
    {
        public string Dados
        {
            get
            {
                return this.richTextBox1.Text;
            }
            set
            {
                this.richTextBox1.Text = value;
            }
        }

        public frmOutros()
        {
            InitializeComponent();
        }
    }
}
