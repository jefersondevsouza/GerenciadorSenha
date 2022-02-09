using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GerenciarSenhas.Regra;

namespace GerenciarSenhas.Interface
{
    public partial class frmNovoUsuario : Form
    {
        Controle controle;

        public frmNovoUsuario()
        {
            InitializeComponent();
            controle = new Controle();
        }

        #region Eventos

        private void btnCriar_Click(object sender, EventArgs e)
        {
            controle.mCriarNovoUsuario(this.txtUsuario.Text, this.txtSenha.Text, this.txtSenhaRepetida.Text, this.txtLembrete.Text);
        }

        #endregion

        #region Métodos


        #endregion

    }
}
