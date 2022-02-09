using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GerenciarSenhas.Interface;
using System.IO;

namespace GerenciarSenhas
{
    static class Program
    {
        public static string pastaBanco = "";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool executarAplicacao = true;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string file = Path.Combine(Application.UserAppDataPath, "LocalArquivoS.txt");

            if (!File.Exists(file))
            {
                File.Create(file).Close();
            }

            pastaBanco = File.ReadAllText(file);
            if (!Directory.Exists(pastaBanco))
            {
                MessageBox.Show("Por favor, defina a pasta onde ficará armazenado o arquivo com as senhas criptografadas.", "Aviso Importante");
                FolderBrowserDialog f = new FolderBrowserDialog();
                f.ShowDialog();
                if (!Directory.Exists(f.SelectedPath))
                {
                    MessageBox.Show("A aplicação será encerrada, pois não foi selecionado uma caminho válido.", "Aviso Importante");
                    executarAplicacao = false;
                }
                else
                {
                    pastaBanco = f.SelectedPath;
                    File.WriteAllText(file, pastaBanco);
                }
            }

            if (executarAplicacao)
            {
                Application.Run(new frmAutentificacao());
            }
        }
    }
}
