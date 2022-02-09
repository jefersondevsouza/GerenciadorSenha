using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GerenciarSenhas.Regra;
using System.Diagnostics;

namespace GerenciarSenhas.Interface
{
    public partial class frmSenhas : Form
    {
        Controle controle;
        string usuario = string.Empty;
        string senha = string.Empty;
        int idUsuario = 0;

        public frmSenhas(string usuario, string senha)
        {
            InitializeComponent();
            controle = new Controle();
            this.usuario = usuario;
            this.senha = senha;

            mCarregarBanco();
        }

        private void mCarregarBanco()
        {
            controle.mCarregarBanco(this.bancoDados);
            this.idUsuario = this.bancoDados.dtbUsuario.Where(u => u.nome.ToUpper().Trim().Equals(usuario.ToUpper().Trim())).First().id;

            this.bindingSource.Filter = "idUsuario = " + this.idUsuario;
            this.bancoDados.dtbSenhas.idUsuarioColumn.DefaultValue = this.idUsuario;
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            this.Refresh();
            controle.mFecharBanco(this.bancoDados);
            controle.mExibirMensagem("Arquivo salvo com sucesso!");
        }

        private void dgvSenhas_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (this.bancoDados.dtbSenhas.Where(bd => bd.idUsuario == 0).Count() > 0)
            {
                foreach (var item in this.bancoDados.dtbSenhas.Where(bd => bd.idUsuario == 0))
                {
                    item.idUsuario = this.idUsuario;
                }
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            controle.mExportarPara();
        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            controle.mImportarPara();
        }

        private void dgvSenhas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                object s = this.dgvSenhas.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                string site = (s ?? "").ToString();
                if (!string.IsNullOrWhiteSpace(site))
                {
                    Process.Start(site);
                }
                
            }
            else if (e.ColumnIndex == 5)
            {
                object s = this.dgvSenhas.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                string informacao = (s ?? "").ToString();

                frmOutros f = new frmOutros();
                f.Dados = informacao;
                f.ShowDialog();

                this.dgvSenhas.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = f.Dados;

            }

        }
    }
}
