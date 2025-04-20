using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimaDevi.Libs;

namespace DimaDevi.Hardware
{
    public class Cache
    {
        public List<string> DeviceID { set; get; }
        public List<int> BlockSize { set; get; }
        public List<Enumerations.CacheType> CacheType { set; get; }
        public List<int> InstalledSize { set; get; }
        public List<int> MaxCacheSize { set; get; }
        public List<int> NumberOfBlocks { set; get; }
        public List<string> Purpose { set; get; }
        public List<Enumerations.WritePolicy> WritePolicy { set; get; }
    }
}
