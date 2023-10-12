using DimaDevi.Libs;
using DimaDevi;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using DimaDevi.Formatters;

namespace DimaDeviTest.Examples
{
    public class SingletonComponent
    {
        /// <summary>
        /// A global component
        /// </summary>
        public SingletonComponent()
        {
            DeviGeneralConfig.GetInstance().AllowSingletonComponents = true; //Set SingletonComponent
            DeviBuild devi = new DeviBuild();
            devi.AddCPU(Enumerations.CPU.Name);
            devi.AddMotherboard();
            devi.AddHash(MD5.Create(), new List<IDeviComponent>() { devi["CPU"], devi["Motherboard"] });
            DeviBuild anotherCoolDevi = new DeviBuild();
            anotherCoolDevi.AddCPU(Enumerations.CPU.Name);
            anotherCoolDevi.AddCPU(Enumerations.CPU.ThreadCount);
            Console.WriteLine(anotherCoolDevi.ToString("<separ>")); //now will print CPU Name, Motherboard, HashMD5 and ThreadCount of CPU. See that have two different DeviBuild but have same output
            Console.WriteLine(devi.ToString(new XmlForm(),"<separ>")); //have exact same output as previous; anotherCoolDevi because of AllowSingletonComponents = true
        }
    }
}
