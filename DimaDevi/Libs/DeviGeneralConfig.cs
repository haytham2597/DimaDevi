using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DimaDevi.Modules;

namespace DimaDevi.Libs
{
    //Singleton
    public class DeviGeneralConfig
    {
        private static DeviGeneralConfig instance;
        
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
        /// <summary>
        /// Exclude "Name=" "ProccessorID=" etc in ToString() 
        /// </summary>
        public bool ExcludeNameComponentString;

        private bool allowSingletonComponents = false;
        /// <summary>
        /// Make all instance of DeviBuild as unique
        /// </summary>
        public bool AllowSingletonComponents
        {
            get => allowSingletonComponents;
            set
            {
                if (value && !allowSingletonComponents) //The private bool is for prevent multiple Debug.WriteLine()
                    Debug.WriteLine("Warning: AllowSingletonComponents is ENABLED");
                allowSingletonComponents = value;
            }
        }
        internal List<string> result = new List<string>();
        internal bool IsObfuscated;
        internal bool PreventDuplicationComponents;
        private DeviGeneralConfig()    
        {
            IsObfuscated = nameof(Base32) == "Base32";
            DeviDefaultSet.GetInstance().AddThis(this);
        }

        /// <summary>
        /// Reset global configuration
        /// </summary>
        public void Reset()
        {
            DeviDefaultSet.GetInstance().SetThis(this);
        }
        public static DeviGeneralConfig GetInstance()
        {
            return instance ?? (instance = new DeviGeneralConfig());
        }
    }
}
