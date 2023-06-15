using System.Collections.Generic;

namespace DimaDevi.Hardware
{
    public sealed class Ram
    {
        public List<long> Capacity { set; get; }
        public List<string> BankLabel { set; get; }
        public List<string>PartNumber { set; get; }
        public List<string> SerialNumber { set; get; }
        public List<string> Speed { set; get; }
    }
}
