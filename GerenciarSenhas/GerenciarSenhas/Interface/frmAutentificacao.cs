using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GerenciarSenhas.Regra;
using System.IO;

namespace GerenciarSenhas.Interface
{
    public partial class frmAutentificacao : Form
    {
        Controle controle;

        public frmAutentificacao()
        {
            InitializeComponent();
            controle = new Controle();
        }

        #region Eventos

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            if (mValidarInformacoes())
            {
                if (controle.mUtilizarUsuario(this.txtUsuario.Text, this.txtSenha.Text, this.txtLembrete))
                {
                    frmSenhas frmSen = new frmSenhas(this.txtUsuario.Text, this.txtSenha.Text);
                    frmSen.ShowDialog();
                }
            }
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            frmNovoUsuario f = new frmNovoUsuario();
            f.ShowDialog(this);
        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            controle.mImportarPara();
        }

        #endregion

        #region Métodos

        private bool mValidarInformacoes()
        {
            if (string.IsNullOrEmpty(this.txtUsuario.Text))
            {
                controle.mExibirMensagem("Informe o usuário!");
                return false;
            }

            if (string.IsNullOrEmpty(this.txtSenha.Text))
            {
                controle.mExibirMensagem("Informe a senha!");
                return false;
            }

            return true;
        }

        #endregion

        private void btnLimparCaminho_Click(object sender, EventArgs e)
        {
            string file = Path.Combine(Application.UserAppDataPath, "LocalArquivoS.txt");

            string caminho = File.ReadAllText(file) + "\\bancoSenha.xjs";

            MessageBox.Show("Caminho (" + caminho + ") será perdido.", "Aviso Importante");
            MessageBox.Show("A aplicação será finalizada, ao reinicia-lá deverá escolher novamente o local.", "Aviso Importante");

            File.WriteAllText(file, "");
            this.Close();
        }

    }
}
