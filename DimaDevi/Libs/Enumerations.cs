using System;
using System.Collections.Generic;

namespace DimaDevi.Libs
{
    public static class Enumerations
    {
        
        public enum DeriveSecure
        {
            PasswordDeriveBytes,
            Rfc2898DeriveBytes
        }

        /// <summary>
        /// For validation
        /// </summary>
        public enum ToleranceLevel
        {
            Reliable,
            Safe,
            Normal,
            Risk,
        }
        public enum TypeHardware
        {
            Unkown,
            CPU,
            Motherboard,
            MacAddress,
            Disk,
            RAM
        }
        [Flags]
        public enum AssemblyEn
        {
            FullName = 1,
            Version = FullName << 1,
            Culture = Version << 1,
            PublicKeyToken = Culture << 1,
            CreationDate = PublicKeyToken << 1,
            /// <summary>
            /// By default MD5
            /// </summary>
            Hash = CreationDate << 1,
            All = ~0
        }

        [Flags]
        public enum GPU
        {
            Name = 1,
            Description = Name << 1,
            DriverDate = Description << 1,
            DriverVersion = DriverDate << 1,
            SystemName = DriverVersion << 1,
            All = ~0
        }
        [Flags]
        public enum CPU
        {
            ProcessorId = 1,
            Name = ProcessorId << 1,
            NumberOfCores = Name << 1,
            Description = NumberOfCores << 1,
            PartNumber = Description << 1,
            ThreadCount = PartNumber << 1,
            All = ~0
        }

        [Flags]
        public enum MacAddress
        {
            Physical = 1,
            Up = Physical << 1, 
            All = Up << 1,
            Hash = All << 1,
        }

        [Flags]
        public enum Motherboard
        {
            Product = 1,
            Name = Product << 1,
            Manufacturer = Name << 1,
            Status = Manufacturer << 1,
            All = ~0
        }

        [Flags]
        public enum RAM
        {
            Capacity = 1,
            BankLabel = Capacity << 1,
            PartNumber = BankLabel << 1,
            SerialNumber = PartNumber << 1,
            Speed = SerialNumber << 1,
            All = ~0
        }

        [Flags]
        public enum Disk
        {
            BytesPerSector = 1,
            Caption = BytesPerSector << 1,
            FirmwareRevision = Caption << 1,
            SerialNumber = FirmwareRevision << 1,
            Model = SerialNumber << 1,
            Size = Model << 1,
            TotalCylinders = Size << 1,
            TotalHeads = TotalCylinders << 1,
            TotalSectors = TotalHeads << 1,
            TotalTracks = TotalSectors << 1,
            /// <summary>
            /// This mean that Only process previously with the Disk of SO not all disk
            /// </summary>
            Main = TotalTracks << 1,
            All = ~0
        }
    }
}
