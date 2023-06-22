using System.Collections.Generic;
using System.Text;
using DimaDevi.Modules;

namespace DimaDevi.Libs
{
    //Singleton
    public class GeneralConfigs
    {
        private static GeneralConfigs instance;

        public RemoteWMICredential RemoteWmi;
        public Encoding Encoding = Encoding.UTF8;
        /// <summary>
        /// Throw messageBox or console if KeySize RSA is greather or equal than 4096
        /// </summary>
        public bool WarningBigKeySizeRSA;
        /// <summary>
        /// Process Components in Runtime While added components from DeviBuild
        /// </summary>
        public bool ProcessComponentsWhileAdd;

        internal List<string> result = new List<string>();
        internal bool IsObfuscated;
        internal bool PreventDuplicationComponents;
        private GeneralConfigs()
        {
            IsObfuscated = nameof(Base32) == "Base32";
            DefaultSet.GetInstance().AddThis(this);
        }

        /// <summary>
        /// Reset global configuration
        /// </summary>
        public void Reset()
        {
            DefaultSet.GetInstance().SetThis(this);
        }
        public static GeneralConfigs GetInstance()
        {
            return instance ?? (instance = new GeneralConfigs());
        }
    }
}
