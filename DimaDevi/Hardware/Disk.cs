using System.Collections.Generic;

namespace DimaDevi.Hardware
{
    public sealed class Disk
    {
        public List<string> DeviceID { set; get; }
        public List<int> BytesPerSector { set; get; }
        public List<string> Caption { set; get; }
        public List<string> FirmwareRevision { set; get; }
        public List<string> SerialNumber { set; get; }
        public List<string> Model { set; get; }
        public List<long> Size { set; get; }
        public List<long> TotalCylinders { set; get; }
        public List<long> TotalHeads { set; get; }
        public List<long> TotalSectors { set; get; }
        public List<long> TotalTracks { set; get; }
    }
}
