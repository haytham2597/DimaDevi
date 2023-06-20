using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows.Forms;
using DimaDevi.Libs;
using DimaDevi.Modules;
using Newtonsoft.Json.Linq;

namespace DimaDevi.Formatters
{
    public class RSAForm : IDeviFormatter
    {
        /// <summary>
        /// XML Public Key
        /// </summary>
        public string PublicKey;

        /// <summary>
        /// XML Private Key
        /// </summary>
        public string PrivateKey;

        public bool usefOAEP = false;
        private void Generate(int KeySize)
        {
            if (KeySize % 2 != 0)
                throw new Exception("Not valid keySize");

            using (var rsa = new RSACryptoServiceProvider(KeySize))
            {
                PrivateKey = rsa.ToXmlString(true);
                PublicKey = rsa.ToXmlString(false);
            }
        }

        private RSAForm()
        {
            DefaultSet.GetInstance().AddThis(this);
        }
        public RSAForm(JObject import) : this()
        {
            var fields = GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
                fields[i].SetValue(this, Convert.ChangeType(import[fields[i].Name], fields[i].FieldType));
        }
        public RSAForm(int keysize = 2048) : this()
        {
            if (keysize >= 4096 && GeneralConfigs.GetInstance().WarningBigKeySizeRSA)
            {
                const string question = "The key is so big, are you sure want continue?";
                if (Environment.UserInteractive)
                {
                    if (MessageBox.Show(question, "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                }
                else
                {
                    Console.WriteLine(question+" Y/N");
                    if (Console.ReadLine()?.ToLower() == "n")
                        return;
                }
            }
            Generate(keysize);
        }

        public string Encrypt(string content, string publicKey)
        {
            const int rest = 11;
            string CipherContent = string.Empty;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                int Formula = ((rsa.KeySize - 384) / 8) + 37;
                int completeSize = Formula + rest; //TODO: Implement Cipher Decipher by block of completeSize instaed of '\n'
                byte[] ReCompute = new byte[] { };
                var bytesContent = GeneralConfigs.GetInstance().Encoding.GetBytes(content);

                int cont = Convert.ToInt32(bytesContent.Length / Formula) + 1;
                int IndexFormula = 0;
                Array.Resize(ref ReCompute, Formula - 1);
                for (int i = 0; i < cont; i++)
                {
                    if (bytesContent.Length < Formula)
                    {
                        var bytesContentCip = rsa.Encrypt(bytesContent, usefOAEP);
                        CipherContent += ((i == 0) ? null : "\n") + Convert.ToBase64String(bytesContentCip);
                        break;
                    }

                    Array.Copy(bytesContent, (IndexFormula == 0) ? 0 : IndexFormula, ReCompute, 0, (IndexFormula == 0) ? Formula - 1 : (i == cont - 1) ? bytesContent.Length - IndexFormula : ReCompute.Length);
                    if (i == cont - 1)
                        Array.Resize(ref ReCompute, Array.FindLastIndex(ReCompute, item => item > 0) + 2);
                    var byteRecompute = rsa.Encrypt(ReCompute, usefOAEP);
                    CipherContent += ((i == 0) ? null : "\n") + Convert.ToBase64String(byteRecompute);
                    IndexFormula += Formula - 1;
                    Array.Clear(ReCompute, 0, ReCompute.Length);
                }
            }
            return CipherContent;
        }

        public string Encrypt(byte[] content)
        {
            return Encrypt(GeneralConfigs.GetInstance().Encoding.GetString(content), PublicKey);
        }

        public string Decrypt(string content)
        {
            string result = string.Empty;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(PrivateKey);
                string[] ler = content.Split('\n');
                foreach (var dd in ler)
                {
                    byte[] bytesCypherText = Convert.FromBase64String(dd);
                    result += GeneralConfigs.GetInstance().Encoding.GetString(rsa.Decrypt(bytesCypherText, false));
                }
            }
            return result;
        }
        
        public string GetDevi(IEnumerable<IDeviComponent> components)
        {
            return Encrypt(components.Joined(), PublicKey);
        }

        public string GetDevi(string componentsResult, string separator)
        {
            return Encrypt(componentsResult, PublicKey);
        }

        public byte[] GetSign(string content)
        {
            using (var RSA_Csp = new RSACryptoServiceProvider())
            {
                RSA_Csp.FromXmlString(PrivateKey);
                return RSA_Csp.SignData(GeneralConfigs.GetInstance().Encoding.GetBytes(content), CryptoConfig.MapNameToOID("SHA512"));
            }
        }

        public bool VerifyData(string original, string signed)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(PrivateKey);
                using (SHA512Managed sha512 = new SHA512Managed())
                {
                    //byte[] hashed = sha512.ComputeHash(Encoding.Unicode.GetBytes(signed));
                    return rsa.VerifyData(GeneralConfigs.GetInstance().Encoding.GetBytes(original), CryptoConfig.MapNameToOID("SHA512"), GeneralConfigs.GetInstance().Encoding.GetBytes(signed));
                }
            }
        }

        public void Dispose()
        {
            DefaultSet.GetInstance().SetThis(this);
        }
    }
}
