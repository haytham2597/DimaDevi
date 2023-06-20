using System;
using System.Collections.Generic;
using DimaDevi.Libs;

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
        private HardwareComponents(){}

        public Dictionary<Type, IList<string>> GetHardware()
        {
            return dicthard;
        }

        public HardwareComponents AddComponent(Type enumType, string field)
        {
            GetInstance().AddComp(enumType, field);
            return GetInstance();
        }
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

            if (dicthard.ContainsKey(enumType))
            {
                dicthard[enumType].Add(field);
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
            AllowException = false;
        }

        public static HardwareComponents GetInstance()
        {
            return instance ?? (instance = new HardwareComponents());
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
