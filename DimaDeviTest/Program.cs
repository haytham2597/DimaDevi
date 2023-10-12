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
            var devi = new DeviBuild();
            /*devi.AddBIOS(Enumerations.BIOS.All);
            devi.AddCPU(Enumerations.CPU.ProcessorId | Enumerations.CPU.Name | Enumerations.CPU.Description);*/
            devi.AddProcess(Process.GetProcessById(18132), Enumerations.ProcessInfo.All);
            Console.WriteLine(devi.ToString());
            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
