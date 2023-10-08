using System;
using System.Collections.Generic;
using System.Text;
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
            BaseType = baseType;
        }
        
        public string Encode(byte[] bytesToEncode)
        {
            return ConvertTo(bytesToEncode);
        }

        public byte[] Decode(string stringToDecode)
        {
             if (stringToDecode.IsBase32())
                 return Base32.FromBase32String(stringToDecode);
             if(stringToDecode.IsHexadecimal())
                 return stringToDecode.DecodeHexadecimal();
             if (stringToDecode.IsBase64())
                 return Convert.FromBase64String(stringToDecode);
             return null;
        }
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
            var bytes = DeviGeneralConfig.GetInstance().Encoding.GetBytes(components.Joined(DeviGeneralConfig.GetInstance().PreventDuplicationComponents));
            return Encode(bytes);
        }
        public string GetDevi(string componentsResult, string separator)
        {
            return Encode(DeviGeneralConfig.GetInstance().Encoding.GetBytes(componentsResult));
        }
        public void Dispose()
        {

        }
    }
}
