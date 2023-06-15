using System.Collections.Generic;

namespace DimaDevi.Libs
{
    internal static class Dict
    {
        public static Dictionary<string, string> DictOfEnum = new Dictionary<string, string>()
        {
            { nameof(Enumerations.RAM), "Win32_PhysicalMemory" },
            { nameof(Enumerations.CPU), "Win32_Processor" },
            { nameof(Enumerations.Motherboard), "Win32_BaseBoard" },
            { nameof(Enumerations.Disk), "Win32_DiskDrive"},
            { nameof(Enumerations.GPU), "Win32_VideoController"}
        };
    }
}
