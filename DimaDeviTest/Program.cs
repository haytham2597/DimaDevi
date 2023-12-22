using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using DimaDevi;
using DimaDevi.Formatters;
using DimaDevi.Hardware;
using DimaDevi.Libs;
using Newtonsoft.Json;

namespace DimaDeviTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /*new Examples.SingletonComponent();
            new Examples.ValidationHardware();*/
            //new Examples.CompressAndDescompress();
            DeviGeneralConfig.GetInstance().ExcludeNameComponentString = true;
            var devi = new DeviBuild().AddMotherboard().AddCPU(Enumerations.CPU.Name).AddMacAddress(Enumerations.MacAddress.Up | Enumerations.MacAddress.Physical);
            //var dla = devi.Components.Select(x => x.GetValue());
            Console.WriteLine(devi.ToString("<blank>"));
            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
