﻿using System;
using System.Diagnostics;
using System.Security.Cryptography;
using DimaDevi;
using DimaDevi.Formatters;
using DimaDevi.Hardware;
using DimaDevi.Libs;

namespace DimaDeviTest
{
    class Program
    {
        static void Main(string[] args)
        {
            HardwareComponents.GetInstance()
                .AddComponent(typeof(Enumerations.CPU), "L3CacheSize")
                .AddComponent(typeof(Enumerations.CPU), "L2CacheSize");
            new Examples.UseAES();
            
            DeviBuild devi = new DeviBuild();
            var start = Stopwatch.GetTimestamp();
            var devicont = devi
                .AddCPU(Enumerations.CPU.Description)
                .AddMachineName()
                .AddMacAddress()
                .AddMotherboard()
                .AddUUID()
                .AddFile(@"K:\LicService.dll", Enumerations.FileInformation.All, new SHA512Managed())
                .AddRam(Enumerations.RAM.All)
                .AddRegistry(@"SOFTWARE\\DefaultUserEnvironment", "Path");
            devicont.Formatter = new BaseXForm(BaseXForm.Base.Hexadecimal);
            string content = devi.ToString("<separ>");
            
            Console.WriteLine(new TimeSpan(Stopwatch.GetTimestamp()-start).ToString());
          
            Console.WriteLine(content);
            Console.WriteLine("-----------");
            Console.WriteLine(devi.DecryptionDecode(content));
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
