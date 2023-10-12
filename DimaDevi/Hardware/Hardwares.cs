using System;
using System.Collections.Generic;
using DimaDevi.Libs;
using DimaDevi.Modules;

namespace DimaDevi.Hardware
{
    public class HardwareComponents
    {
        private static HardwareComponents instance;
        private readonly Dictionary<Type, IList<string>> dicthard = new Dictionary<Type, IList<string>>();
        /// <summary>
        /// Throw exception if put bad Type enum
        /// </summary>
        public bool AllowException;

        private HardwareComponents()
        {
            DefaultSet.GetInstance().AddThis(this);
        }
        public static HardwareComponents GetInstance()
        {
            return instance ?? (instance = new HardwareComponents());
        }
        public Dictionary<Type, IList<string>> GetHardware()
        {
            return dicthard;
        }

        public HardwareComponents AddComponent(Type enumType, string field)
        {
            this.AddComp(enumType, field);
            return this;
        }

        /*public HardwareComponents AddComponent(string hardware, string field)
        {
        }

        public void AddHardware(string hardware, string wmiclass)
        {
            if (Libs.Dict.WMIClass.ContainsKey(hardware))
                return;

            Libs.Dict.WMIClass.Add(hardware, wmiclass);
        }*/
        private void AddComp(Type enumType, string field)
        {
            if (!enumType.IsEnum)
            {
                if (AllowException)
                    throw new Exception("Not valid enumeration");
                return;
            }
            if(!Dict.WMIClass.ContainsKey(enumType.Name))
                throw new Exception("This enum type do not exists");

            if (dicthard.TryGetValue(enumType, out var value))
            {
                value.Add(field);
                return;
            }
            dicthard.Add(enumType, new List<string>(){field});
        }

        /// <summary>
        /// Clear all hardwares and Reset configuration to default
        /// </summary>
        public void Reset()
        {
            dicthard.Clear();
            DefaultSet.GetInstance().SetThis(this);
        }

    }
    public sealed class Hardwares
    {
        public CPU Cpu { set; get; }
        public Disk Disk { set; get; }
        public Motherboard Motherboard { set; get; }
        public Ram Ram { set; get; }
        public MacAddress MacAddress { set; get; }
        /*public Hardwares(string import)
        {
            try
            {
                JObject jo = JObject.Parse(import);
                if (jo.IsEqualProperty(this.GetType()))
                {
                    var prop = JsonConvert.DeserializeObject<Hardwares>(import);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }*/
    }
}
