using System;
using DimaDevi;
using DimaDevi.Formatters;
using DimaDevi.Libs;

namespace DimaDeviTest.Examples
{
    public class CommonFormatter
    {
        public CommonFormatter()
        {
            DeviBuild devi = new DeviBuild();
            devi.AddGPU(Enumerations.GPU.All).AddMacAddress().AddMotherboard(Enumerations.Motherboard.All);
            devi.Formatter = new HashForm();
            Console.WriteLine($"Hash: {devi.ToString("#")}");
            devi.Formatter = new JsonForm();
            Console.WriteLine($"Json: {devi.ToString("#")}");
            devi.Formatter = new XmlForm();
            Console.WriteLine($"Xml: {devi.ToString("#")}");
            devi.Dispose();
            
        }
    }
}
