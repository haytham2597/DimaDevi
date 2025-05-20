using DimaDevi.Formatters;
using DimaDevi.Libs;
using DimaDevi;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using DimaDevi.Hardware;

namespace DimaDeviTest.Examples
{
    public class ValidationHardware
    {
        //Warning: Need fixed...
        public ValidationHardware()
        {
            HardwareComponents.GetInstance()
                .AddComponent(typeof(Enumerations.CPU), "L3CacheSize")
                .AddComponent(typeof(Enumerations.CPU), "L2CacheSize");
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

            Console.WriteLine(new TimeSpan(Stopwatch.GetTimestamp() - start).ToString());

            Console.WriteLine(content);
            Console.WriteLine("-----------");
            Console.WriteLine(devi.DecryptionDecode(content));
            DeviBuild devi1 = new DeviBuild().AddCPU().AddMacAddress();
            Console.WriteLine("Validation Percentage: " + devi.Validate(devi1.GetHardwares()));
        }
    }
}
