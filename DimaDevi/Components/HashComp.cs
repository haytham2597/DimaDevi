using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DimaDevi.Libs;
using DimaDevi.Libs.Extensions;

namespace DimaDevi.Components
{
    public class HashComp : IDeviComponent
    {
        public Func<string, string> Replacement { get; set; }
        public string BaseHardware { get; set; } = "Hash";
        public string Name { get; } = "Hash";
        private readonly HashAlgorithm Hash;
        private readonly string Content;
        public HashComp(HashAlgorithm hash, string content)
        {
            Hash = hash;
            Content = content;
        }

        public HashComp(HashAlgorithm hash, IList<IDeviComponent> components) : this(hash, string.Join(",",components.Select(x=>x.GetValue()).ToArray())) { }
        public string GetValue()
        {
            string result = Convert.ToBase64String(Hash.ComputeHash(DeviGeneralConfig.GetInstance().Encoding.GetBytes(Content)));
            if (Replacement != null)
                return Replacement(result);
            return result;
        }
        public bool Equals(IDeviComponent other)
        {
            return this.EqualsObject(other);
        }
    }
}
