using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;

namespace GerenciarSenhas.Regra
{
    class CriptografiaDados
    {
        private string arquivoEntrada = string.Empty;
        private string arquivoSaida = string.Empty;
        private string pChaveCrip = string.Empty;

        public CriptografiaDados()
        {
        }

        public static void mCriptografarArquivo(string sConteudoArquivo, string sOutputFilename, string sKey, bool deleteOrigem) 
        {
            using (FileStream fsInput = new FileStream(sConteudoArquivo, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsEncrypted = new FileStream(sOutputFilename, FileMode.Create, FileAccess.Write))
                {
                    DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
                    DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                    DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                    ICryptoTransform desencrypt = DES.CreateEncryptor();
                    using (CryptoStream cryptostream = new CryptoStream(fsEncrypted, desencrypt, CryptoStreamMode.Write))
                    {
                        byte[] bytearrayinput = new byte[fsInput.Length];
                        fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
                        cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
                    }
                }
            }

            if (deleteOrigem)
                mDeletarArquivo(sConteudoArquivo);
        }

        public static void mDescriptografarArquivo(string sInputFilename, string sOutputFilename, string sKey, bool deleteOrigem)
        {
            using (DESCryptoServiceProvider DES = new DESCryptoServiceProvider())
            {
                //A 64 bit key and IV is required for this provider.
                //Set secret key For DES algorithm.
                DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                //Set initialization vector.
                DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

                //Create a file stream to read the encrypted file back.
                using (FileStream fsread = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read))
                {
                    //Create a DES decryptor from the DES instance.
                    ICryptoTransform desdecrypt = DES.CreateDecryptor();
                    //Create crypto stream set to read and do a 
                    //DES decryption transform on incoming bytes.
                    using (CryptoStream cryptostreamDecr = new CryptoStream(fsread, desdecrypt, CryptoStreamMode.Read))
                    {
                        //Print the contents of the decrypted file.
                        using (StreamWriter fsDecrypted = new StreamWriter(sOutputFilename))
                        {
                            fsDecrypted.Write(new StreamReader(cryptostreamDecr).ReadToEnd());
                            fsDecrypted.Flush();
                        }
                    }
                }
            }

            if (deleteOrigem)
                mDeletarArquivo(sInputFilename);
        }

        private static void mDeletarArquivo(string arquivo)
        {
            if (File.Exists(arquivo))
            {
                File.Delete(arquivo);
            }
        }

        public static void mCriptografarConteudo(string sConteudoArquivo, string arquivoCriptografado, string chave)
        {
            using (FileStream fsEncrypted = new FileStream(arquivoCriptografado, FileMode.Create, FileAccess.Write))
            {
                using (DESCryptoServiceProvider DES = new DESCryptoServiceProvider())
                {
                    DES.Key = ASCIIEncoding.ASCII.GetBytes(chave);
                    DES.IV = ASCIIEncoding.ASCII.GetBytes(chave);
                    ICryptoTransform desencrypt = DES.CreateEncryptor();
                    using (CryptoStream cryptostream = new CryptoStream(fsEncrypted, desencrypt, CryptoStreamMode.Write))
                    {
                        byte[] bytearrayinput = System.Text.ASCIIEncoding.ASCII.GetBytes(sConteudoArquivo);
                        cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
                    }
                }
            }
        }

        public static string mDescriptografarArquivo(string sInputFilename, string sKey)
        {
            string retorno = "";
            using (DESCryptoServiceProvider DES = new DESCryptoServiceProvider())
            {
                DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

                //Create a file stream to read the encrypted file back.
                using (FileStream fsread = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read))
                {
                    //Create a DES decryptor from the DES instance.
                    ICryptoTransform desdecrypt = DES.CreateDecryptor();
                    //Create crypto stream set to read and do a 
                    //DES decryption transform on incoming bytes.
                    using (CryptoStream cryptostreamDecr = new CryptoStream(fsread, desdecrypt, CryptoStreamMode.Read))
                    {
                        using (StreamReader s = new StreamReader(cryptostreamDecr))
                        {
                            retorno = s.ReadToEnd();
                        }
                    }
                }
            }

            return retorno;
        }


        public static string mGerarChave()
        {
            DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();

            // Use the Automatically generated key for Encryption. 
            string chave = ASCIIEncoding.ASCII.GetString(desCrypto.Key);
            return chave;
        }
    }
}
