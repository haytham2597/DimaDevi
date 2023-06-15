using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using DimaDevi.Libs;
using DimaDevi.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DimaDevi.Formatters
{
    public sealed class AESForm : IDeviFormatter
    {
        public bool PreventComponentDuplication { get; set; }
        private readonly string Password;
        public int Iterations { set; get; } = 50000;
        public CipherMode Cipher { set; get; }
        public PaddingMode Padding { set; get; }
        public Enumerations.DeriveSecure DeriveSecure { set; get; } = Enumerations.DeriveSecure.Rfc2898DeriveBytes;
        public RijndaelManaged Managed { set; get; }
        public byte[] Salt { set; get; }
        private static RijndaelManaged InitManaged(DeriveBytes key, CipherMode cipher, PaddingMode padding)
        {
            RijndaelManaged AES = new RijndaelManaged { KeySize = 256, BlockSize = 128 };
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = padding;
            AES.Mode = cipher;
            return AES;
        }

        public AESForm(JObject import)
        {
            var props = this.GetType().GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                if (props[i].PropertyType == typeof(RijndaelManaged))
                {
                    var m = JsonConvert.DeserializeObject<RijndaelManaged>(import[props[i].Name].ToString());
                    if (m != null)
                        props[i].SetValue(this, m);
                    continue;
                }
                props[i].SetValue(this, Convert.ChangeType(import[props[i].Name], props[i].PropertyType));
            }
        }
        /// <summary>
        /// This method including SALT in content
        /// </summary>
        /// <param name="password"></param>
        /// <param name="size">128,196,256</param>
        /// <param name="cipher"></param>
        /// <param name="padding"></param>
        public AESForm(string password, int size = 256, CipherMode cipher = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            Password = password;
            Cipher = cipher;
            Padding = padding;
            this.Salt = Ext.GenerateRandomSalt();
            Managed = InitManaged(DeriveSecure == Enumerations.DeriveSecure.PasswordDeriveBytes ? (DeriveBytes)new PasswordDeriveBytes(Password, Salt, "SHA1", Iterations) : new Rfc2898DeriveBytes(Password, Salt, Iterations), Cipher, Padding);
        }

        public AESForm(ElipticCurveDiffieHellman ecdh, CipherMode cipher = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            Cipher = cipher;
            Padding = padding;
            Managed = new RijndaelManaged { KeySize = 256, BlockSize = 128 };
            Managed.GenerateIV();
            Managed.Key = ecdh.GetDerivate();
            Managed.IV = Managed.IV;
            Managed.Padding = this.Padding;
            Managed.Mode = cipher;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Encrypt(byte[] content)
        {
            if (Managed == null)
                Managed = InitManaged(DeriveSecure == Enumerations.DeriveSecure.PasswordDeriveBytes ? (DeriveBytes)new PasswordDeriveBytes(Password, Salt, "SHA1", Iterations) : new Rfc2898DeriveBytes(Password, Salt, Iterations), Cipher, Padding);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, Managed.CreateEncryptor(Managed.Key, Managed.IV), CryptoStreamMode.Write))
                    cs.Write(content, 0, content.Length);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Decrypt(string content)
        {
            var cont = Convert.FromBase64String(content);
            if(Managed == null)
                Managed = InitManaged(DeriveSecure == Enumerations.DeriveSecure.PasswordDeriveBytes ? (DeriveBytes)new PasswordDeriveBytes(Password, Salt, "SHA1", Iterations) : new Rfc2898DeriveBytes(Password, Salt, Iterations), Cipher, Padding);
            var decryptor = Managed.CreateEncryptor(Managed.Key, Managed.IV); //El modo CFB y otros usa encryptor
            if (this.Cipher == CipherMode.CBC ^ this.Cipher == CipherMode.ECB) //Los unicos modos que usan Descryptor
                decryptor = Managed.CreateDecryptor(Managed.Key, Managed.IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    cs.Write(cont, 0, cont.Length);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        public void SetIV(byte[] iv)
        {
            if (Managed != null)
                Managed.IV = iv;
        }
        public byte[] GetIV()
        {
            return Managed.IV;
        }
        public byte[] GetSalt()
        {
            return this.Salt;
        }

        public string GetDevi(IEnumerable<IDeviComponent> components)
        {
            var bytes = Encoding.UTF8.GetBytes(components.Joined(PreventComponentDuplication));
            return Encrypt(bytes);
        }
        
        public string GetDevi(string componentsResult, string separator)
        {
            return Encrypt(Encoding.UTF8.GetBytes(componentsResult));
        }
        /// <summary>
        /// Export all configuration of this
        /// </summary>
        /// <returns></returns>
        [Obfuscation(Feature ="all")]
        public string Export()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Import configuration 
        /// </summary>
        /// <param name="content">Json value</param>
        [Obfuscation(Feature ="all")]
        public void Import(string content)
        {
            var parse = JObject.Parse(content);
            var prop = this.GetType().GetProperties();
            foreach (var c in parse)
            {
                for (int i = 0; i < prop.Length; i++)
                {
                    if (c.Key != prop[i].Name)
                        continue;
                    if (c.Key == prop[i].Name && prop[i].PropertyType == typeof(RijndaelManaged))
                    {
                        var m = JsonConvert.DeserializeObject<RijndaelManaged>(c.Value?.ToString());
                        if(m != null)
                            prop[i].SetValue(this, m);
                        continue;
                    }
                    prop[i].SetValue(this, Convert.ChangeType(c.Value, prop[i].PropertyType));
                }
            }
        }
        public void Dispose()
        {
            this.RandomizedStringDispose();
        }
    }
}
