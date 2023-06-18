using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
            this.IsObfuscated = nameof(DeviBuild) == "DeviBuild";
        }

        /// <summary>
        /// Reset global configuration
        /// </summary>
        public void Reset()
        {
            /*var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].IsPrivate)
                    continue;
                if(fields[i].FieldType == typeof(bool))
                    fields[i].SetValue(this, false);
                //TODO: Check if field is class
                //TODO: Check if field is list generic
            }*/
            WarningBigKeySizeRSA = false;
            ProcessComponentsWhileAdd = false;
            Encoding = Encoding.UTF8;
            RemoteWmi.Dispose();
            result.Clear();
            PreventDuplicationComponents = false;
        }
        public static GeneralConfigs GetInstance()
        {
            return instance ?? (instance = new GeneralConfigs());
        }
    }
}
