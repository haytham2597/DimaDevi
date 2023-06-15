using System;
using DimaDevi.Libs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DimaDevi.Hardware
{
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
