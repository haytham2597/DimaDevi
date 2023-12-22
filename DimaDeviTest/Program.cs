using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using DimaDevi;
using DimaDevi.Components;
using DimaDevi.Formatters;
using DimaDevi.Hardware;
using DimaDevi.Libs;
using Newtonsoft.Json;
using static DimaDevi.Libs.Enumerations;

namespace DimaDeviTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /*new Examples.SingletonComponent();
            new Examples.ValidationHardware();*/
            //new Examples.CompressAndDescompress();
            var nc = new NetworkComp(Enumerations.MacAddress.Physical | Enumerations.MacAddress.Up) { PreventVPN = true };
            var valalal = nc.GetValue();
            var wm = new WMIComp("NetAdapterWithoutVPN", "Win32_NetworkAdapter", "MACAddress, PNPDeviceID", "PNPDeviceID", "PNPDeviceID LIKE \"PCI%\"");
            var vals = wm.GetValues();
            if (vals.All(x => x.ContainsKey("MACAddress")))
            {
                var macs = vals.Select(x => x["MACAddress"].ToString().Replace(":", ""));
                var RealGetPhysicalAddress = NetworkInterface.GetAllNetworkInterfaces().AsEnumerable().Where(x => macs.Contains(x.GetPhysicalAddress().ToString()));
            }

            Console.WriteLine();
            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
