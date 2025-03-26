using System;
using System.Collections.Generic;
using System.Text;
using DimaDevi.Libs;
using DimaDevi.Libs.Extensions;
using DimaDevi.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DimaDevi.Formatters
{
    public sealed class ChaCha20Form : IDeviFormatter
    {
        private ChaCha20 chacha20;
        public byte[] Key;
        public byte[] Nonce;
        public readonly uint Counter = 1;
        private readonly bool PasswordGenerator;
        
        //TODO: Save Nonce, Key and Counter... Because in decryption need them.
        private ChaCha20Form() { }
        public ChaCha20Form(JObject import)
        {
            var fields = GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
                fields[i].SetValue(this, Convert.ChangeType(import[fields[i].Name], fields[i].FieldType));
        }
        public ChaCha20Form(string password, int nonceSize = 12)
        {
            var p = Encoding.UTF8.GetBytes(password);
            if (p.Length > 32 || p.Length <= 0)
                Array.Resize(ref p, 32);
            Key = p;
            Nonce = CommonExt.GenerateRandomSalt(nonceSize);
            Init();
        }

        public ChaCha20Form(string password, string nonce)
        {
            var p = Encoding.UTF8.GetBytes(password);
            var n = Encoding.UTF8.GetBytes(nonce);
            if (p.Length > 32)
                Array.Resize(ref p, 32);
            if(n.Length > 12)
                Array.Resize(ref n, 12);
            Key = p;
            Nonce = n;
            Init();
        }

        public ChaCha20Form(int keySize = 32, int nonceSize = 12)
        {
            Key = CommonExt.GenerateRandomSalt(keySize);
            Nonce = CommonExt.GenerateRandomSalt(nonceSize);
            PasswordGenerator = true;
            Init();
        }

        private void Init()
        {
            chacha20 = new ChaCha20(Key, Nonce, Counter);
            
        }
        private string Encrypt(string content)
        {
            var cont = DeviGeneralConfig.GetInstance().Encoding.GetBytes(content);
            if(PasswordGenerator)
                cont = Key.Combine(cont);
            byte[] encryptedContent = new byte[cont.Length];
            chacha20.EncryptBytes(encryptedContent, cont);
            return Convert.ToBase64String(encryptedContent);
        }

        public string Decrypt(string content)
        {
            chacha20 = new ChaCha20(Key, Nonce, 1); // In decryption the ChaCha20 flush Key and Nonce bytes after decryption. So Reload if the user call this method again
            var cont = Convert.FromBase64String(content);
            byte[] decryptContent = new byte[cont.Length];
            chacha20.DecryptBytes(decryptContent, cont);
            return DeviGeneralConfig.GetInstance().Encoding.GetString(decryptContent);
        }
        
        public string GetDevi(IEnumerable<IDeviComponent> components)
        {
            return Encrypt(components.Joined(DeviGeneralConfig.GetInstance().PreventDuplicationComponents));
        }

        public string GetDevi(string componentsResult, string separator)
        {
            return Encrypt(componentsResult);
        }

        public string Export()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void Import(string str)
        {
            var parse = JObject.Parse(str);
            var prop = GetType().GetProperties();
            foreach (var c in parse)
            {
                for (int i = 0; i < prop.Length; i++)
                {
                    if (c.Key != prop[i].Name)
                        continue;
                    if (prop[i].PropertyType == typeof(ChaCha20))
                        continue;
                    prop[i].SetValue(this, Convert.ChangeType(c.Value, prop[i].PropertyType));
                }
            }
        }

        public void Dispose()
        {
            chacha20.Dispose();
            //this.RandomizedStringDispose();
        }
    }
}
