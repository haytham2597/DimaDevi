using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimaDevi.Libs;
using DimaDevi.Modules;

namespace DimaDevi.Formatters
{
    public sealed class BaseXForm : IDeviFormatter
    {
        public enum Base
        {
            Base32,
            Base64,
            Hexadecimal
        }

        private readonly Base BaseType;
        public BaseXForm(Base baseType = Base.Base64)
        {
            this.BaseType = baseType;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public string Encode(byte[] bytesToEncode)
        {
            return ConvertTo(bytesToEncode);
        }

        public byte[] Decode(string stringToDecode)
        {
             //Check if string is Base64, 32 (with regex) and Hexadecimal
             if (stringToDecode.IsBase32())
                 return Base32.FromBase32String(stringToDecode);
             if(stringToDecode.IsHexadecimal())
                 return stringToDecode.DecodeHexadecimal();
             if (stringToDecode.IsBase64())
                 return Convert.FromBase64String(stringToDecode);
             return null;
        }
        //Encode, Decode

        private string ConvertTo(byte[] bytes)
        {
            if (BaseType == Base.Base32)
                return Base32.ToBase32String(bytes);
            if (BaseType == Base.Hexadecimal)
            {
                var sb = new StringBuilder();
                foreach (var t in bytes)
                    sb.Append(t.ToString("X2"));
                return sb.ToString();
            }
            return Convert.ToBase64String(bytes);
        }
        public string GetDevi(IEnumerable<IDeviComponent> components)
        {
            var bytes = GeneralConfigs.GetInstance().Encoding.GetBytes(components.Joined(GeneralConfigs.GetInstance().PreventDuplicationComponents));
            return Encode(bytes);
        }

        public string GetDevi(string componentsResult, string separator)
        {
            return Encode(GeneralConfigs.GetInstance().Encoding.GetBytes(componentsResult));
        }
    }
}
