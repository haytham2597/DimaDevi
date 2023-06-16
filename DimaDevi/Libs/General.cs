using System.Text;

namespace DimaDevi.Libs
{
    //Singleton
    public class General
    {
        private static General instance;
        public Property.RemoteWMICredential RemoteWmi;
        public Encoding Encoding { set; get; } = Encoding.UTF8;

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
