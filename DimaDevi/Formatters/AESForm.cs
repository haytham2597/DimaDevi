using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using DimaDevi.Libs;
using DimaDevi.Libs.Extensions;
using DimaDevi.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DimaDevi.Formatters
{
    public sealed class AESForm : Imports, IDeviFormatter
    {
        private readonly string Password;
        private int iterations = 50000;
        public int Iterations
        {
            set
            {
                if (value < 1000)
                {
                    iterations = 1000;
                    return;
                }
                iterations = value;
            }
            get => iterations;
        }

        public CipherMode Cipher;
        public PaddingMode Padding;
        public Enumerations.DeriveSecure DeriveSecure { set; get; } = Enumerations.DeriveSecure.Rfc2898DeriveBytes;
        public RijndaelManaged Managed { set; get; }
        public byte[] Salt { set; get; }
        private DeriveBytes deriveBytes = null;
        private RijndaelManaged InitManaged(DeriveBytes key, CipherMode cipher, PaddingMode padding)
        {
            this.deriveBytes = key;
            RijndaelManaged AES = new RijndaelManaged { KeySize = 256, BlockSize = 128 };
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = padding;
            AES.Mode = cipher;
            return AES;
        }

        private AESForm()
        {
            DefaultSet.GetInstance().AddThis(this);
        }
        public AESForm(JObject import) : this()
        {
            var props = GetType().GetProperties();
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
        public AESForm(string password, int size = 256, CipherMode cipher = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7) : this()
        {
            Password = password;
            Cipher = cipher;
            Padding = padding;
            Salt = CommonExt.GenerateRandomSalt();
            Managed = InitManaged(DeriveSecure == Enumerations.DeriveSecure.PasswordDeriveBytes ? (DeriveBytes)new PasswordDeriveBytes(Password, Salt, "SHA1", Iterations) : new Rfc2898DeriveBytes(Password, Salt, Iterations), Cipher, Padding);
        }

        public AESForm(ElipticCurveDiffieHellman ecdh, CipherMode cipher = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7) : this()
        {
            Cipher = cipher;
            Padding = padding;
            Managed = new RijndaelManaged { KeySize = 256, BlockSize = 128 };
            Managed.GenerateIV();
            Managed.Key = ecdh.GetDerivate();
            Managed.Padding = Padding;
            Managed.Mode = cipher;
            Salt = CommonExt.GenerateRandomSalt();
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Attrs.MethodName("Encrypt")]
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Attrs.MethodName("Decrypt")]
        public string Decrypt(string content)
        {
            var cont = Convert.FromBase64String(content);
            if(Managed == null)
                Managed = InitManaged(DeriveSecure == Enumerations.DeriveSecure.PasswordDeriveBytes ? (DeriveBytes)new PasswordDeriveBytes(Password, Salt, "SHA1", Iterations) : new Rfc2898DeriveBytes(Password, Salt, Iterations), Cipher, Padding);
            var decryptor = Managed.CreateDecryptor(Managed.Key, Managed.IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    cs.Write(cont, 0, cont.Length);
                return DeviGeneralConfig.GetInstance().Encoding.GetString(ms.ToArray());
            }
        }
        public void SetIV(byte[] iv)
        {
            if (Managed != null)
                Managed.IV = iv;
        }

        public void SetSaltWithDerivate(string salt)
        {
            var by = DeviGeneralConfig.GetInstance().Encoding.GetBytes(salt);
            DeriveBytes deriv = DeriveSecure == Enumerations.DeriveSecure.PasswordDeriveBytes ? (DeriveBytes)new PasswordDeriveBytes(Password, by, "SHA1", Iterations) : new Rfc2898DeriveBytes(Password, by, Iterations);
            Managed = InitManaged(deriv, Cipher, Padding);
        }
        public byte[] GetIV()
        {
            return Managed.IV;
        }
        public byte[] GetSalt()
        {
            return Salt;
        }

        public string GetDevi(IEnumerable<IDeviComponent> components)
        {
            var bytes = DeviGeneralConfig.GetInstance().Encoding.GetBytes(components.Joined(DeviGeneralConfig.GetInstance().PreventDuplicationComponents));
            return Encrypt(bytes);
        }
        
        public string GetDevi(string componentsResult, string separator)
        {
            return Encrypt(DeviGeneralConfig.GetInstance().Encoding.GetBytes(componentsResult));
        }
        /// <summary>
        /// Export all configuration of this
        /// </summary>
        /// <returns></returns>
        public string Export()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Import configuration 
        /// </summary>
        /// <param name="content">Json value</param>
        public void Import(string content)
        {
            var parse = JObject.Parse(content);
            var prop = GetType().GetProperties();
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

        public void Clear()
        {
            DefaultSet.GetInstance().SetThis(this);
        }
        public void Dispose()
        {
            Clear();
            //this.RandomizedStringDispose();
        }
    }
}
