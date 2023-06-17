using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using DimaDevi.Libs;

namespace DimaDevi.Components
{
    public sealed class FileComp : IDeviComponent, IDisposable
    {
        private readonly HashAlgorithm hash = MD5.Create(); //Md5 because is more faster than Sha256 and security is not needed, so just use MD5 For only HASH file
        public string BaseHardware { set; get; } = nameof(FileComp);
        public string Name { get; } = "FileHash";
        public Enumerations.FileInformation FileInfomation = Enumerations.FileInformation.Name;
        private readonly string FilePath;
        public FileComp(string path)
        {
            FilePath = path;
            if (File.Exists(FilePath))
                Name += Path.GetFileNameWithoutExtension(FilePath);
        }

        public FileComp(string path, HashAlgorithm hashAlg) : this(path)
        {
            if (hashAlg == null)
                return;
            hash = hashAlg;
        }

        public FileComp(string path, Enumerations.FileInformation fileInformation, HashAlgorithm hashAlg) : this(path, hashAlg)
        {
            this.FileInfomation = fileInformation;
        }
        public string GetValue()
        {
            if (!File.Exists(FilePath))
                return null;
            FileInfo file = new FileInfo(FilePath);
            List<byte[]> bytes = new List<byte[]>();
            if (FileInfomation.HasFlag(Enumerations.FileInformation.Attributes))
                bytes.Add(GeneralConfigs.GetInstance().Encoding.GetBytes(file.Attributes.ToString()));
            if(FileInfomation.HasFlag(Enumerations.FileInformation.Name))
                bytes.Add(GeneralConfigs.GetInstance().Encoding.GetBytes(file.Name));
            if (FileInfomation.HasFlag(Enumerations.FileInformation.CreationDate))
                bytes.Add(GeneralConfigs.GetInstance().Encoding.GetBytes(file.CreationTime.ToString("s")));
            if (FileInfomation.HasFlag(Enumerations.FileInformation.ModifiedDate))
                bytes.Add(GeneralConfigs.GetInstance().Encoding.GetBytes(file.LastWriteTime.ToString("s")));

            if (FileInfomation.HasFlag(Enumerations.FileInformation.Content))
            {
                byte[] buffer = new byte[file.Length];
                if (file.Length > 1024 * 1024) //1mb
                    Array.Resize(ref buffer, 1024 * 1024);
                using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) //FileShare ReadAndWrite for prevent exception of locked file
                    fs.Read(buffer, 0, buffer.Length);
                bytes.Add(buffer);
            }
            return Convert.ToBase64String(hash.ComputeHash(bytes.SelectMany(x => x).ToArray()));
        }

        public void Dispose()
        {
            this.hash.Dispose();
        }
    }
}
