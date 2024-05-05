using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using DimaDevi.Libs;
using DimaDevi.Libs.Extensions;
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
            DeviDefaultSet.GetInstance().AddThis(this);
        }
        public RSAForm(JObject import) : this()
        {
            var fields = GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
                fields[i].SetValue(this, Convert.ChangeType(import[fields[i].Name], fields[i].FieldType));
        }
        public RSAForm(int keysize = 2048) : this()
        {
            if (keysize >= 4096 && DeviGeneralConfig.GetInstance().WarningBigKeySizeRSA)
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
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                int ChunkSize = rsa.GetCompleteSize();
                int Formula = ChunkSize - 11;
                int FormulaBlock = Formula - 1;
                var BytesContent = DeviGeneralConfig.GetInstance().Encoding.GetBytes(content);
                int Cont = Convert.ToInt32(BytesContent.Length / Formula) + 1;
                byte[] Encrypted = new byte[ChunkSize * Cont];
                for (int i = 0; i < Cont; i++)
                {
                    byte[] Compute = new byte[FormulaBlock];
                    Buffer.BlockCopy(BytesContent, i * FormulaBlock, Compute, 0, (i == Cont - 1) ? BytesContent.Length - (i * FormulaBlock) : Compute.Length);
                    var encry = rsa.Encrypt(Compute, false);
                    Buffer.BlockCopy(encry, 0, Encrypted, ChunkSize * i, encry.Length);
                }
                return Convert.ToBase64String(Encrypted);
            }
        }

        public string Encrypt(byte[] content)
        {
            return Encrypt(DeviGeneralConfig.GetInstance().Encoding.GetString(content), PublicKey);
        }

        public string Decrypt(string content)
        {
            string result = string.Empty;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(this.PrivateKey);
                int ChunkSize = rsa.GetCompleteSize();
                byte[] cipherContent = Convert.FromBase64String(content);
                int Cont = cipherContent.Length / ChunkSize;
                for (int i = 0; i < Cont; i++)
                {
                    byte[] buf = new byte[ChunkSize];
                    Buffer.BlockCopy(cipherContent, i * ChunkSize, buf, 0, ChunkSize);
                    result += DeviGeneralConfig.GetInstance().Encoding.GetString(rsa.Decrypt(buf, false));
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
                return RSA_Csp.SignData(DeviGeneralConfig.GetInstance().Encoding.GetBytes(content), CryptoConfig.MapNameToOID("SHA512"));
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
                    return rsa.VerifyData(DeviGeneralConfig.GetInstance().Encoding.GetBytes(original), CryptoConfig.MapNameToOID("SHA512"), DeviGeneralConfig.GetInstance().Encoding.GetBytes(signed));
                }
            }
        }

        public void Dispose()
        {
            DeviDefaultSet.GetInstance().SetThis(this);
        }
    }
}
