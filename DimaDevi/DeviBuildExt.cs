using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using DimaDevi.Components;
using DimaDevi.Libs;
using DimaDevi.Libs.Extensions;
using Microsoft.Win32;

namespace DimaDevi
{
    /// <summary>
    /// Fluent DeviBuild
    /// </summary>
    public static class DeviBuildExt
    {
        public static DeviBuild AddComponents(this DeviBuild devi, IDeviComponent deviComponent)
        {
            devi.Components.Add(deviComponent);
            if (DeviGeneralConfig.GetInstance().ProcessComponentsWhileAdd)
                DeviGeneralConfig.GetInstance().result.Add(deviComponent.Name + "="+deviComponent.GetValue());
            return devi;
        }
        
        //[Obfuscation(ApplyToMembers = true, Feature = "all")]
        private static DeviBuild AddByFlags(this DeviBuild devi, IEnumerable<Enum> flags)
        {
            using (var enumer = flags.GetEnumerator())
            {
                while (enumer.MoveNext())
                {
                    if (Convert.ToInt32(enumer.Current) == -1) //is ALL
                        continue;

                    var last = enumer.Current?.GetType().Name;
                    if (string.IsNullOrEmpty(last) || !Dict.WMIClass.ContainsKey(last))
                        continue;
                    devi.AddComponents(new WMIComp(enumer.Current?.ToString(), Dict.WMIClass[last], enumer.Current?.ToString()){BaseHardware = last });
                }
            }
            return devi;
        }

        public static DeviBuild AddPrevOperation(this DeviBuild devi, Func<string,string> func)
        {
            devi.Components[devi.Components.Count - 1].Replacement = func;
            return devi;
        }

        public static DeviBuild AddBIOS(this DeviBuild devi, Enumerations.BIOS bios)
        {
            var enumType = bios.GetType();
            var gca = enumType.GetCustomAttributes(typeof(Attrs.WMINameAttribute), true);
            var flags = bios.GetFlags();
            using (var enumer = flags.GetEnumerator())
            {
                while (enumer.MoveNext())
                {
                    if (Convert.ToInt32(enumer.Current) == -1) //is ALL
                        continue;
                    if (enumer.Current == null)
                        continue;
                    var memberInfos = enumType.GetMember(enumer.Current.ToString());
                    var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                    var valueAttributes = enumValueMemberInfo?.GetCustomAttributes(typeof(Attrs.WMINameAttribute), false);
                    devi.AddComponents(new WMIComp(enumer.Current.ToString(), (gca[0] as Attrs.WMINameAttribute)?.Name, valueAttributes?.Length == 0 ? enumer.Current.ToString() : (valueAttributes?[0] as Attrs.WMINameAttribute)?.Name) { BaseHardware = enumType.Name });
                }
            }
            return devi;
        }
        public static DeviBuild AddMachineName(this DeviBuild devi)
        {
            return devi.AddComponents(new DeviComp("MachineName", Environment.MachineName){BaseHardware = "Environment"});
        }
        public static DeviBuild AddUserName(this DeviBuild devi)
        {
            return devi.AddComponents(new DeviComp("UserName", Environment.UserName){BaseHardware = "Environment"});
        }
        public static DeviBuild AddOSVersion(this DeviBuild devi)
        {
            return devi.AddComponents(new DeviComp("OSVersion", Environment.OSVersion.ToString){BaseHardware = "Environment"});
        }
        public static DeviBuild AddCustom(this DeviBuild devi, string name, Func<string> func_)
        {
            return devi.AddComponents(new DeviComp(name, func_) { BaseHardware = "Custom" });
        }
        public static DeviBuild AddCustom(this DeviBuild devi, string name, Func<string> func_, string baseHardwareName)
        {
            return devi.AddComponents(new DeviComp(name, func_) { BaseHardware = baseHardwareName });
        }
        public static DeviBuild AddCustom(this DeviBuild devi, IDeviComponent deviComp)
        {
            return devi.AddComponents(deviComp);
        }
        public static DeviBuild AddMacAddress(this DeviBuild devi, Enumerations.MacAddress macAddress = Enumerations.MacAddress.All)
        {
            return devi.AddComponents(new NetworkComp(macAddress));
        }
        public static DeviBuild AddMacAddress(this DeviBuild devi, string ip, Enumerations.MacAddress macAddress = Enumerations.MacAddress.All)
        {
            return devi.AddComponents(new NetworkComp(ip, macAddress));
        }
        public static DeviBuild AddMacAddress(this DeviBuild devi, IPAddress ip, Enumerations.MacAddress macAddress = Enumerations.MacAddress.All)
        {
            return devi.AddComponents(new NetworkComp(ip, macAddress));
        }
        public static DeviBuild AddMacAddress(this DeviBuild devi, Enumerations.MacAddress macAddress, IList<NetworkInterfaceType> networkInterfaces)
        {
            return devi.AddComponents(new NetworkComp(macAddress, networkInterfaces));
        }
        public static DeviBuild AddMacAddress(this DeviBuild devi, Enumerations.MacAddress macAddress, IList<NetworkInterfaceType> networkInterfaces, IList<OperationalStatus> operationalStatus)
        {
            return devi.AddComponents(new NetworkComp(macAddress, networkInterfaces, operationalStatus));
        }
        public static DeviBuild AddCPU(this DeviBuild devi, Enumerations.CPU cpus = Enumerations.CPU.ProcessorId)
        {
            return devi.AddByFlags(cpus.GetFlags());
        }
        public static DeviBuild AddRam(this DeviBuild devi, Enumerations.RAM rams = Enumerations.RAM.PartNumber)
        {
            return devi.AddByFlags(rams.GetFlags());
        }
        public static DeviBuild AddMotherboard(this DeviBuild devi, Enumerations.Motherboard mothers = Enumerations.Motherboard.Product)
        {
            return devi.AddByFlags(mothers.GetFlags());
        }
        public static DeviBuild AddFile(this DeviBuild devi, string path, HashAlgorithm hash = null)
        {
            return devi.AddComponents(new FileComp(path, hash));
        }
        public static DeviBuild AddFile(this DeviBuild devi, string path, Enumerations.FileInformation fileInformation, HashAlgorithm hash = null)
        {
            return devi.AddComponents(new FileComp(path, fileInformation, hash));
        }
        /// <summary>
        /// Add itself assembly
        /// </summary>
        /// <param name="devi"></param>
        /// <param name="assemblyEn"></param>
        /// <returns></returns>
        public static DeviBuild AddSelf(this DeviBuild devi, Enumerations.AssemblyEn assemblyEn = Enumerations.AssemblyEn.PublicKeyToken)
        {
            return devi.AddComponents(new AssemblyComp(Assembly.GetEntryAssembly(), assemblyEn));
        }
        /// <summary>
        /// Add Assembly
        /// </summary>
        /// <param name="devi"></param>
        /// <param name="assembly"></param>
        /// <param name="assemblyEn"></param>
        /// <returns></returns>
        public static DeviBuild AddAssembly(this DeviBuild devi, Assembly assembly, Enumerations.AssemblyEn assemblyEn = Enumerations.AssemblyEn.PublicKeyToken)
        {
            return devi.AddComponents(new AssemblyComp(assembly, assemblyEn));
        }

        public static DeviBuild AddGPU(this DeviBuild devi, Enumerations.GPU gpu = Enumerations.GPU.Name)
        {
            return devi.AddByFlags(gpu.GetFlags());
        }
        public static DeviBuild AddDisk(this DeviBuild devi, Enumerations.Disk disk = Enumerations.Disk.FirmwareRevision)
        {
            if (disk.HasFlag(Enumerations.Disk.Main))
            {
                var flags=disk.GetFlags();
                using (var enumer = flags.GetEnumerator())
                {
                    while (enumer.MoveNext())
                    {
                        if (enumer.Current == null)
                            continue;
                        if ((Enumerations.Disk)enumer.Current == Enumerations.Disk.Main)
                            continue;
                        var te = enumer.Current?.GetType().Name;
                        if (te == null)
                            continue;
                        if (!Dict.WMIClass.ContainsKey(te))
                            continue;
                        devi.AddComponents(new WMIComp(enumer.Current?.ToString(),  Dict.WMIClass[te], "*", enumer.Current?.ToString(), $"DeviceID={CommonExt.GetMainPhysicalDriveOS()}"));
                    }
                }
                return devi;
            }
            return devi.AddByFlags(disk.GetFlags());
        }

        public static DeviBuild AddRegistry(this DeviBuild devi, string base_key, string name_key)
        {
            return devi.AddComponents(new RegistryComp(base_key, name_key));
        }

        public static DeviBuild AddRegistry(this DeviBuild devi, RegistryKey reg_key, string base_key, string name_key)
        {
            return devi.AddComponents(new RegistryComp(reg_key, base_key, name_key));
        }

        public static DeviBuild AddRegistry(this DeviBuild devi, string base_key, string name_key, RegistryHive reg_hive = RegistryHive.LocalMachine, RegistryView reg_view = RegistryView.Registry32)
        {
            return devi.AddComponents(new RegistryComp(reg_hive, reg_view, base_key, name_key));
        }

        public static DeviBuild AddUUID(this DeviBuild devi)
        {
            return devi.AddComponents(new WMIComp("UUID", "Win32_ComputerSystemProduct", "UUID"));
        }

        public static DeviBuild AddMachineUUID(this DeviBuild devi)
        {
            return devi.AddRegistry(@"SOFTWARE\\Microsoft\\Cryptography", "MachineGuid");
        }

        public static DeviBuild AddHash(this DeviBuild devi, HashAlgorithm hash, IList<IDeviComponent> components)
        {
            return devi.AddComponents(new HashComp(hash, components));
        }

        public static DeviBuild AddHash(this DeviBuild devi, HashAlgorithm hash, string content)
        {
            return devi.AddComponents(new HashComp(hash, content));
        }
    }
}
