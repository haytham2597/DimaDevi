using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimaDevi.Libs
{
    //Singleton
    public class General
    {
        private static General instance;
        private Encoding encoding = null;
        public Encoding Encoding
        {
            set => encoding = value;
            get => encoding ?? (encoding = Encoding.UTF8);
        }

        internal bool IsObfuscated;
        private General()
        {
            this.IsObfuscated = nameof(DeviBuild) == "DeviBuild";
        }

        public static General GetInstance()
        {
            return instance ?? (instance = new General());
        }
    }
}
