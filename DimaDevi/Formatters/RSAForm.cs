using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows.Forms;
using DimaDevi.Libs;
using Newtonsoft.Json.Linq;

namespace DimaDevi.Formatters
{
    public class RSAForm : IDeviFormatter
    {
        public bool PreventComponentDuplication { get; set; }
        public string PublicKey;
        public string PrivateKey;
        private void Generate(int KeySize)
        {
            if (KeySize % 2 != 0)
                throw new Exception("Not valid keySize");

            using (var RSA_CSP = new RSACryptoServiceProvider(KeySize))
            {
                /*var privateKey = RSA_CSP.ExportParameters(true);
                var publicKey = RSA_CSP.ExportParameters(false);*/
                PrivateKey = RSA_CSP.ToXmlString(true);
                PublicKey = RSA_CSP.ToXmlString(false);
                /*string publicKeyString;
                {
                    var sw = new StringWriter();
                    var xs = new XmlSerializer(typeof(RSAParameters));
                    xs.Serialize(sw, publicKey);
                    publicKeyString = sw.ToString();
                }
                publicKeyString = publicKeyString.RemoveDeclarationXml();
                RSA_CSP = new RSACryptoServiceProvider();
                RSA_CSP.ImportParameters(privateKey);
                PrivateKey = RSA_CSP.ToXmlString(true);
                PublicKey = publicKeyString;*/
            }
        }

        public RSAForm(JObject import)
        {
            var fields = this.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
                fields[i].SetValue(this, Convert.ChangeType(import[fields[i].Name], fields[i].FieldType));
        }
        public RSAForm(int keysize = 2048)
        {
            if (keysize >= 4096)
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

        public static string Encrypt(string content, string publicKey)
        {
            string CipherContent = string.Empty;
            using (var RSA_Csp = new RSACryptoServiceProvider())
            {
                RSA_Csp.FromXmlString(publicKey);
                int Formula = ((RSA_Csp.KeySize - 384) / 8) + 37;
                byte[] ReCompute = new byte[] { };
                var bytesContent = General.GetInstance().Encoding.GetBytes(content);

                int cont = Convert.ToInt32(bytesContent.Length / Formula) + 1;
                int IndexFormula = 0;
                Array.Resize(ref ReCompute, Formula - 1);
                for (int i = 0; i < cont; i++)
                {
                    if (bytesContent.Length < Formula)
                    {
                        var bytesContentCip = RSA_Csp.Encrypt(bytesContent, false);
                        CipherContent += ((i == 0) ? null : "\n") + Convert.ToBase64String(bytesContentCip);
                        break;
                    }

                    Array.Copy(bytesContent, (IndexFormula == 0) ? 0 : IndexFormula, ReCompute, 0, (IndexFormula == 0) ? Formula - 1 : (i == cont - 1) ? bytesContent.Length - IndexFormula : ReCompute.Length);
                    if (i == cont - 1)
                        Array.Resize(ref ReCompute, Array.FindLastIndex(ReCompute, item => item > 0) + 2);
                    var byteRecompute = RSA_Csp.Encrypt(ReCompute, false);
                    CipherContent += ((i == 0) ? null : "\n") + Convert.ToBase64String(byteRecompute);
                    IndexFormula += Formula - 1;
                    Array.Clear(ReCompute, 0, ReCompute.Length);
                }
            }

            return CipherContent;
        }

        public string Encrypt(byte[] content)
        {
            return Encrypt(General.GetInstance().Encoding.GetString(content), this.PublicKey);
        }

        public string Decrypt(string content)
        {
            var csp = new RSACryptoServiceProvider();
            csp.FromXmlString(this.PrivateKey);
            string[] ler = content.Split('\n');
            string Result = string.Empty;
            foreach (var dd in ler)
            {
                byte[] bytesCypherText = Convert.FromBase64String(dd);
                Result += General.GetInstance().Encoding.GetString(csp.Decrypt(bytesCypherText, false));
            }
            return Result;
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
                RSA_Csp.FromXmlString(this.PrivateKey);
                return RSA_Csp.SignData(General.GetInstance().Encoding.GetBytes(content), CryptoConfig.MapNameToOID("SHA512"));
            }
        }

        public bool VerifyData(string original, string signed)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(this.PrivateKey);
                using (SHA512Managed sha512 = new SHA512Managed())
                {
                    //byte[] hashed = sha512.ComputeHash(Encoding.Unicode.GetBytes(signed));
                    return rsa.VerifyData(General.GetInstance().Encoding.GetBytes(original), CryptoConfig.MapNameToOID("SHA512"), General.GetInstance().Encoding.GetBytes(signed));
                }
            }
        }

        public void Dispose()
        {
            this.RandomizedStringDispose();
        }
    }
}
