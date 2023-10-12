using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DimaDevi.Libs.Extensions;

namespace DimaDevi.Modules
{
    public class IPMacMapper : IDisposable
    {
        private static List<IPAndMac> list;
        private class IPAndMac
        {
            public string IP { get; set; }
            public string MAC { get; set; }
        }
        private StreamReader ExecuteCommandLine(string file, string arguments = "")
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = file;
            startInfo.Arguments = arguments;
            Process process = Process.Start(startInfo);
            return process?.StandardOutput;
        }

        private void InitializeGetIPsAndMac()
        {
            if (list != null)
                return;

            var arpStream = ExecuteCommandLine("arp", "-a");
            List<string> result = new List<string>();
            while (!arpStream.EndOfStream)
            {
                var line = arpStream.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(line))
                    continue;
                result.Add(line);

            }
            list = result.Select(x => {
                string[] parts = x.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                return new IPAndMac { IP = parts[0].Trim(), MAC = parts[1].Trim() };
            }).ToList();
        }

        public string Find(string macOrIp)
        {
            return macOrIp.IsPossibleIP() ? FindMacFromIPAddress(macOrIp) : FindIPFromMacAddress(macOrIp);
        }

        public string FindIPFromMacAddress(string macAddress)
        {
            InitializeGetIPsAndMac();
            IPAndMac item = list.SingleOrDefault(x => x.MAC == macAddress);
            return item?.IP;
        }

        public string FindMacFromIPAddress(string ip)
        {
            InitializeGetIPsAndMac();
            IPAndMac item = list.SingleOrDefault(x => x.IP == ip);
            return item?.MAC;
        }

        public void Dispose()
        {
            list.Clear();
        }
    }
}
