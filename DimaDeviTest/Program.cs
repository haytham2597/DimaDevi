using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DimaDevi;
using DimaDevi.Formatters;
using DimaDevi.Hardware;
using DimaDevi.Libs;
using DimaDevi.Modules;
using Microsoft.Win32;

namespace DimaDeviTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var h = HardwareComponents.GetInstance();
            
            //h.AddComponent(typeof(Enumerations.CPU), "L3CacheSize");
            h.AddComponent(typeof(Enumerations.CPU), "L3CacheSize").AddComponent(typeof(Enumerations.CPU), "L2CacheSize");
            DimaDevi.DeviBuild devi = new DeviBuild();
            var start = Stopwatch.GetTimestamp();
            var devicont = devi
                .AddCPU(Enumerations.CPU.Name)
                .AddMachineName()
                .AddMacAddress()
                .AddMotherboard()
                .AddGPU(Enumerations.GPU.All);
            string content = devi.ToString("<separ>");
            Console.WriteLine(new TimeSpan(Stopwatch.GetTimestamp()-start).ToString());
            /*var bob = new ElipticCurveDiffieHellman();
            var alice = new ElipticCurveDiffieHellman(bob.GetPublicKey());
            bob.AddPublic(alice);
            
            var aes = new AESForm(alice);
            DimaDevi.DeviBuild devi = new DeviBuild();
            var devicont = devi
                    .AddCPU(Enumerations.CPU.All)
                    .AddDisk(Enumerations.Disk.Caption | Enumerations.Disk.Main | Enumerations.Disk.FirmwareRevision)
                    .AddGPU(Enumerations.GPU.All)
                    .AddDisk(Enumerations.Disk.FirmwareRevision | Enumerations.Disk.SerialNumber | Enumerations.Disk.Size | Enumerations.Disk.Model)
                    .AddUserName()
                    .AddMachineName()
                    .AddMacAddress();
            string content = devi.ToString( "#\n");
            var aes1 = new AESForm(bob.AddPublic(alice));*/

            Console.WriteLine(content);
            Console.WriteLine("-----------");
            DeviBuild devi1 = new DeviBuild().AddCPU().AddMacAddress();
            Console.WriteLine("Validation Percentage: "+devi.Validate(devi1.GetHardwares()));
            /*var decr = devi.Decryption(content, aes1);
            Console.WriteLine(decr);*/
            /*var f = devi.GetInfoFormatter();
            Console.WriteLine("Prev form :"+devi.Formatter.GetType().FullName);
            devi.Formatter = aes;
            Console.WriteLine("After set manually form :" + devi.Formatter.GetType().FullName);
            devi.ImportFormatter(f);

            Console.WriteLine("Actual form: "+devi.Formatter.GetType().FullName);
            Console.WriteLine("Encrypt: " + content);*/
            /*Console.WriteLine("-----");
            var aesB = new AESForm(bob.AddPublic(alice.GetPublicKey()));
            aesB.SetIV(aes.GetIV());
            devi = new DeviBuild(aesB);
            Console.WriteLine("Decryption: " + devi.Decryption(content));*/
            //Console.WriteLine("Decryption: "+devi.Decryption("qOnSVHUs1iNZeebI1kMtlf6Au04N4V53A5O0LQNmzJodMh2mzs/hkcxBpupze5qGkrK5NNH7V0W29GYZ+sUqT0a5r4ug8ccq9hUlsaJZYik="));
            //Console.WriteLine(devi.ToString("#\n"));

            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
