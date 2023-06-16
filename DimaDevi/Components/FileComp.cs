using System;
using System.IO;
using System.Security.Cryptography;

namespace DimaDevi.Components
{
    public sealed class FileComp : IDeviComponent, IDisposable
    {
        private readonly HashAlgorithm hash = MD5.Create(); //Md5 because is more faster than Sha256 and security is not needed, so just use MD5 For only HASH file
        public string BaseHardware { set; get; } = nameof(FileComp);
        public string Name { get; } = "FileHash";
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
        public string GetValue()
        {
            if (!File.Exists(FilePath))
                return null;
            FileInfo file = new FileInfo(FilePath);
            byte[] buffer = new byte[file.Length];
            if (file.Length > 1024 * 1024) //1mb
                Array.Resize(ref buffer, 1024 * 1024);
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) //FileShare ReadAndWrite for prevent exception of locked file
                fs.Read(buffer, 0, buffer.Length);
            return Convert.ToBase64String(hash.ComputeHash(buffer));
        }

        public void Dispose()
        {
            this.hash.Dispose();
        }
    }
}
