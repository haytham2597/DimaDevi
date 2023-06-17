using System.Text;

namespace DimaDevi.Libs
{
    //Singleton
    public class GeneralConfigs
    {
        private static GeneralConfigs instance;
        public Property.RemoteWMICredential RemoteWmi;
        public Encoding Encoding { set; get; } = Encoding.UTF8;

        internal bool IsObfuscated;
        internal bool PreventDuplicationComponents;
        private GeneralConfigs()
        {
            this.IsObfuscated = nameof(DeviBuild) == "DeviBuild";
        }

        public static GeneralConfigs GetInstance()
        {
            return instance ?? (instance = new GeneralConfigs());
        }
    }
}
