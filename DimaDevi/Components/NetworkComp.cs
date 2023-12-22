using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using DimaDevi.Libs;
using DimaDevi.Libs.Extensions;
using DimaDevi.Modules;

namespace DimaDevi.Components
{
    public sealed class NetworkComp : IDeviComponent
    {
        public Func<string, string> Replacement { get; set; }
        public string BaseHardware { set; get; } = nameof(Hardware.MacAddress);
        public string Name => "MacAddress";
        private Enumerations.MacAddress MacAddresses { set; get; }
        private IList<NetworkInterfaceType> NetworkInterfaceList { set; get; }
        private IList<OperationalStatus> OperationalStatusList { set; get; }
        private readonly string IP;
        public bool PreventVPN = true;
        public NetworkComp()
        {

        }

        /// <summary>
        /// Get MAC from IP
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="macAddress"></param>
        public NetworkComp(string ip, Enumerations.MacAddress macAddress)
        {
            IP = ip;
        }

        public NetworkComp(IPAddress ip, Enumerations.MacAddress macAddress) : this(ip.ToString(), macAddress) { }

        public NetworkComp(Enumerations.MacAddress macAddress)
        {
            MacAddresses = macAddress;
        }
        public NetworkComp(Enumerations.MacAddress macAddress, IList<NetworkInterfaceType> netInterfaces) : this(macAddress)
        {
            NetworkInterfaceList = netInterfaces;
        }
        public NetworkComp(Enumerations.MacAddress macAddress, IList<NetworkInterfaceType> netInterfaces, IList<OperationalStatus> opStatus) : this(macAddress, netInterfaces)
        {
            OperationalStatusList = opStatus;
        }

        private IEnumerable<NetworkInterface> GetInter()
        {
            var inter = NetworkInterface.GetAllNetworkInterfaces().AsEnumerable();
            if (MacAddresses.HasFlag(Enumerations.MacAddress.All) || NetworkInterfaceList != null)
            {
                if (NetworkInterfaceList != null && NetworkInterfaceList.Count != 0)
                    inter = inter.Where(x => NetworkInterfaceList.Contains(x.NetworkInterfaceType));
                if (OperationalStatusList != null && OperationalStatusList.Count != 0)
                    inter = inter.Where(x => OperationalStatusList.Contains(x.OperationalStatus));
            }
            return inter;
        }
        private Func<string, string> ResultReplacement()
        {
            return s =>
            {
                if (Replacement != null)
                    return Replacement(MacAddresses.HasFlag(Enumerations.MacAddress.Hash) ? s.ToMD5Base64() : s);
                return MacAddresses.HasFlag(Enumerations.MacAddress.Hash) ? s.ToMD5Base64() : s;
            };
        }
        public string GetValue()
        {
            var funcReplacement = ResultReplacement();
            var inter = GetInter();
            //TODO: Detect VPN
            if (PreventVPN)
            {
                var wm = new WMIComp("NetAdapterWithoutVPN", "Win32_NetworkAdapter", "MACAddress, PNPDeviceID", "PNPDeviceID", "PNPDeviceID LIKE \"PCI%\"");
                var vals = wm.GetValues();
                if (vals.All(x => x.ContainsKey("MACAddress")))
                {
                    var macs = vals.Select(x => x["MACAddress"].ToString().Replace(":",""));
                    var RealGetPhysicalAddress = NetworkInterface.GetAllNetworkInterfaces().AsEnumerable().Where(x => macs.Contains(x.GetPhysicalAddress().ToString())).Select(x=>x.GetPhysicalAddress().ToString());
                    return funcReplacement(RealGetPhysicalAddress.FirstOrDefault());
                }
            }

            if (!string.IsNullOrEmpty(IP))
            {
                using (var ipmac = new IPMacMapper())
                {
                    var mac = ipmac.FindMacFromIPAddress(IP);
                    return MacAddresses.HasFlag(Enumerations.MacAddress.Hash) ? mac.ToMD5Base64() : mac;
                }
            }
            if (MacAddresses.HasFlag(Enumerations.MacAddress.All) || NetworkInterfaceList != null)
            {
                var interStr = inter.Select(x => x.GetPhysicalAddress().ToString()).Where(x => x != "000000000000" && x != "00000000000000E0").Select(x => x.FormatMacAddress()).ToList();
                var result = interStr.Count > 0 ? string.Join(",", interStr) : null;
                funcReplacement(result);
            }
            var macAddr = inter.Select(x => x.GetPhysicalAddress().ToString()).FirstOrDefault();
            return funcReplacement(macAddr);
        }
        public bool Equals(IDeviComponent other)
        {
            return this.EqualsObject(other);
        }
    }
}
