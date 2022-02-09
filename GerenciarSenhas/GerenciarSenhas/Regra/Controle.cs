using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GerenciarSenhas.AcessoDados;
using System.Security.Cryptography;
using System.Data;

namespace GerenciarSenhas.Regra
{
    class Controle
    {
        #region Atributos

        //String pasta = System.Environment.CurrentDirectory.ToString();
        private String pLocalPadrao = Program.pastaBanco;//@"C:\GerenciadorSenhas";
        private const string pNomeBancoCript = @"bancoSenha.xjs";
        private const string pNomeBancoXml = @"bancoSenha.xml";
        private string pChaveCrip = @"senha123";//informar aqui uma chave aqui que será utilizar para criptografar as senhas.

        #endregion

        #region Construtores

        public Controle()
        {
        }

        #endregion

        #region Métodos

        public DialogResult mExibirMensagem(string mensagem)
        {
            return MessageBox.Show(mensagem, "Gerenciador de Senhas", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public DialogResult mExibirMensagemPergunta(string mensagem)
        {
            return MessageBox.Show(mensagem, "Gerenciador de Senhas", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
        }

        public bool mUtilizarUsuario(string usuario, string senha, TextBox lembrete)
        {
            BancoDados.dtbUsuarioRow drwUsuario = mVerificarUsuario(usuario);
            if (drwUsuario != null)
            {
                if (drwUsuario.senha == senha)
                    return true;
                else
                {
                    this.mExibirMensagem("A senha infomada está errada.");
                    lembrete.Text = drwUsuario.IslembreteSenhaNull()? "" : drwUsuario.lembreteSenha;
                    return false;
                }
            }
            else
            {
                this.mExibirMensagem("O usuário informado não existe no banco de dados!");
                return false;
            }
        }

        public bool mCriarNovoUsuario(string usuario, string senha, string senhaRepetida, string lembreteSenha)
        {
            BancoDados.dtbUsuarioRow drwUsuario = mVerificarUsuario(usuario);
            if (drwUsuario == null)
            {
                if (!senha.Equals(senhaRepetida))
                {
                    this.mExibirMensagem("As senhas não são iguais!");
                    return false;
                }

                if (string.IsNullOrEmpty(lembreteSenha))
                {
                    this.mExibirMensagem("Informe o lembrete de senha!");
                    return false;
                }


                BancoDados banco = new BancoDados();
                this.mCarregarBanco(banco);

                banco.dtbUsuario.AdddtbUsuarioRow(usuario, senha, lembreteSenha);
                banco.AcceptChanges();
                this.mFecharBanco(banco);
                this.mExibirMensagem("O usuário " + usuario + " foi criado com sucesso!");

                return true;
            }
            else
            {
                this.mExibirMensagem("O usuário informado já existe no banco de dados!");
                if (this.mExibirMensagemPergunta("Deseja atualizar o lembrete de senha deste usuário?") == DialogResult.Yes)
                {
                    if (drwUsuario != null)
                    {
                        if (!senha.Equals(senhaRepetida))
                        {
                            this.mExibirMensagem("As senhas não são iguais!");
                            return false;
                        }

                        if (string.IsNullOrEmpty(lembreteSenha))
                        {
                            this.mExibirMensagem("Informe o lembrete de senha!");
                            return false;
                        }


                        BancoDados banco = new BancoDados();
                        this.mCarregarBanco(banco);
                        var user = banco.dtbUsuario.Where(u => u.nome.Equals(usuario)).ToList();

                        if (user != null && user.Count == 1)
                        {
                            user.First().lembreteSenha = lembreteSenha;

                            banco.AcceptChanges();
                            this.mFecharBanco(banco);
                            this.mExibirMensagem("O usuário " + usuario + " atualizado com sucesso!");
                            return true;

                        }
                        else
                        {
                            if (user != null && user.Count > 1)
                            {
                                this.mExibirMensagem("Mais de um usuário encontrado com esse nome!");
                            }
                        }
                    }
                }

                return false;
            }
        }

        public void mExportarPara()
        {
            string local = mSelecionarLocal();
            if (!local.Equals(string.Empty) && Directory.Exists(local))
            {
                mCopiarDePara(pLocalPadrao, local);
                this.mExibirMensagem("Banco exportado com sucesso!!");
            }
        }

        public void mImportarPara()
        {

            if (MessageBox.Show("Quando você realizar a importação do banco, perderá as senhas antigas. \nMesmo assim deseja continuar a importação?",
                "Gerenciador de Senhas", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {

                string local = mSelecionarLocal();
                if (!local.Equals(string.Empty) && Directory.Exists(local))
                {
                    mCopiarDePara(local, pLocalPadrao);
                    this.mExibirMensagem("Banco importado com sucesso!!");
                }
            }
        }

        public void mCarregarBanco(DataSet banco)
        {
            if (File.Exists(pLocalPadrao + "\\" + pNomeBancoCript))
            {
                mDescompactarArquivo();
                mLerArquivoCriptografado(banco);
            }
            else
            {
                banco = new BancoDados();
            }
        }

        public void mFecharBanco(BancoDados banco)
        {
            string conteudo = mGerarArquivoXml(banco);
            mCompactarArquivo(conteudo);
        }

        private BancoDados.dtbUsuarioRow mVerificarUsuario(string usuario)
        {
            BancoDados banco = new BancoDados();
            this.mCarregarBanco(banco);

            foreach (var drwUsuario in banco.dtbUsuario)
            {
                if (drwUsuario.nome.ToUpper().Trim().Equals(usuario.ToUpper().Trim()))
                    return drwUsuario;
            }

            return null;
        }

        private void mCopiarDePara(string origem, string destino)
        {
            try
            {
                File.Copy(origem + "\\" + pNomeBancoCript,
                        destino + "\\" + pNomeBancoCript,
                        true);
            }
            catch (Exception ex)
            {
                this.mExibirMensagem(ex.Message);
            }
        }

        private string mGerarArquivoXml(BancoDados banco)
        {
            string nomeCompletoArquivo = pLocalPadrao + "\\" + pNomeBancoXml;

            //StringBuilder xml = new StringBuilder("");

            //using (StreamWriter lArqXML = new StreamWriter(nomeCompletoArquivo))
            //{
            //    xml.Append(banco.GetXmlSchema());
            //    xml.Append(
            //    "<<<SCHEMA_XML>>>");
            //    xml.Append(banco.GetXml());
            //    lArqXML.Write(xml);
            //    lArqXML.Close();
            //}

            StringBuilder xml = new StringBuilder("");

            xml.Append(banco.GetXmlSchema());
            xml.Append(
            "<<<SCHEMA_XML>>>");
            xml.Append(banco.GetXml());

            return xml.ToString();
        }

        private void mLerXml(DataSet dataSet)
        {
            // DataSet dataSet = new DataSet();
            string nomeCompletoArquivo = pLocalPadrao + "\\" + pNomeBancoXml;
            string delimitador = "<<<SCHEMA_XML>>>";

            StreamReader sr = new StreamReader(nomeCompletoArquivo);
            StringBuilder xml = new StringBuilder(sr.ReadToEnd());
            StringReader schema = new StringReader(xml.ToString().Substring(0, xml.ToString().LastIndexOf(delimitador)));
            StringReader dados = new StringReader(xml.ToString().Substring(xml.ToString().LastIndexOf(delimitador) + delimitador.Length));
            dataSet.ReadXmlSchema(schema);
            dataSet.ReadXml(dados);
            dataSet.AcceptChanges();
            sr.Close();

        }

        private void mLerArquivoCriptografado(DataSet dataSet)
        {
            string delimitador = "<<<SCHEMA_XML>>>";

            string conteudo = this.mDescompactarArquivo();

            StringBuilder xml = new StringBuilder(conteudo);
            StringReader schema = new StringReader(xml.ToString().Substring(0, xml.ToString().LastIndexOf(delimitador)));
            StringReader dados = new StringReader(xml.ToString().Substring(xml.ToString().LastIndexOf(delimitador) + delimitador.Length));
            dataSet.ReadXmlSchema(schema);
            dataSet.ReadXml(dados);
            dataSet.AcceptChanges();

        }

        private void mCompactarArquivo(string conteudo)
        {
            string caminhoCript = pLocalPadrao + "\\" + pNomeBancoCript;

            try
            {
                CriptografiaDados.mCriptografarConteudo(conteudo, caminhoCript, this.pChaveCrip);
            }
            catch (Exception ex)
            {
                this.mExibirMensagem(ex.Message);
            }

        }

        private string mDescompactarArquivo()
        {
            string caminhoCript = pLocalPadrao + "\\" + pNomeBancoCript;

            try
            {
                return CriptografiaDados.mDescriptografarArquivo(caminhoCript, this.pChaveCrip);
            }
            catch (Exception ex)
            {
                this.mExibirMensagem(ex.Message);
            }

            return "";
        }

        private string mSelecionarLocal()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                return fbd.SelectedPath;
            }
            return string.Empty;
        }

        #endregion

    }
}
