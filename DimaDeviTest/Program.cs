using System;
using DimaDevi;
using DimaDevi.Hardware;
using DimaDevi.Libs;
using Newtonsoft.Json;

namespace DimaDeviTest
{
    class Program
    {
        /*[DllImport("cpuid.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cpuid_vec(ref int len);

        private static int[] GetArrayInt(IntPtr ptr, int len, int offset = 0)
        {
            int[] res = new int[len];
            Marshal.Copy(ptr, res, offset, len);
            return res;
        }*/
        static void Main(string[] args)
        {
            /*int len = 0;
            IntPtr v = cpuid_vec(ref len);
            int[] vv = GetArrayInt(v, len);
            var cpuid = new CPUID(vv);*/
            var cpuid = new CPUID();

            Console.WriteLine(JsonConvert.SerializeObject(cpuid, Formatting.Indented));
            /*new Examples.SingletonComponent();
            new Examples.ValidationHardware();*/
            //new Examples.CompressAndDescompress();
            //new Examples.UseRSA();
            var devi =new DeviBuild();
            devi.AddCache(Enumerations.Cache.All);
            foreach (var g in devi.ToGroup())
            {
                Console.WriteLine($"Group: {g.Key}");
                using (var enumer = g.GetEnumerator())
                {
                    while (enumer.MoveNext())
                    {
                        if (enumer.Current == null)
                            continue;
                        Console.WriteLine($"- Name: {enumer.Current.Name}, Value: {enumer.Current.GetValue()}");
                    }
                }
            }
            /*var nc = new NetworkComp(Enumerations.MacAddress.Physical | Enumerations.MacAddress.Up) { PreventVPN = true };
            var valalal = nc.GetValue();
            var wm = new WMIComp("NetAdapterWithoutVPN", "Win32_NetworkAdapter", "MACAddress, PNPDeviceID", "PNPDeviceID", "PNPDeviceID LIKE \"PCI%\"");
            var vals = wm.GetValues();
            if (vals.All(x => x.ContainsKey("MACAddress")))
            {
                var macs = vals.Select(x => x["MACAddress"].ToString().Replace(":", ""));
                var RealGetPhysicalAddress = NetworkInterface.GetAllNetworkInterfaces().AsEnumerable().Where(x => macs.Contains(x.GetPhysicalAddress().ToString()));
            }*/

            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
