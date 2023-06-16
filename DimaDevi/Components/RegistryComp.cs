using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace DimaDevi.Components
{
    public class RegistryComp : IDeviComponent
    {
        public string BaseHardware { get; set; } = "Registry";
        public string Name { get; } = "Registry";

        private RegistryKey RegKey;
        private readonly string BaseKey;
        private readonly string NameKey;
        private readonly RegistryHive RegHive;
        private RegistryView RegView = RegistryView.Registry32;
        /// <summary>
        /// RegKey is LocalMachine
        /// </summary>
        /// <param name="base_key"></param>
        /// <param name="name_key"></param>
        public RegistryComp(string base_key, string name_key)
        {
            this.BaseKey = base_key;
            this.NameKey = name_key;
            RegKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
        }
        public RegistryComp(RegistryKey reg_key, string base_key, string name_key) : this(base_key, name_key)
        {
            this.RegKey = reg_key;
            this.BaseKey = base_key;
            this.NameKey = name_key;
        }
        public RegistryComp(RegistryHive reg_hive, RegistryView reg_view, string base_key, string name_key) : this(base_key, name_key)
        {
            this.RegHive = reg_hive;
            this.RegView = reg_view;
            this.BaseKey = base_key;
            this.NameKey = name_key;
            RegKey = RegistryKey.OpenBaseKey(reg_hive, reg_view);
        }
        public RegistryComp(string name, RegistryKey reg_key, string base_key, string name_key) : this(reg_key, base_key,name_key)
        {
            Name = name;
        }
       
        public string GetValue()
        {
            try
            {
                if (RegKey == null)
                    RegKey = RegistryKey.OpenBaseKey(RegHive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
                RegistryKey key = RegKey.OpenSubKey(this.BaseKey);
                object o = key?.GetValue(this.NameKey, "default");
                return o?.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
