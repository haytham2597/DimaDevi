using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            new Examples.SingletonComponent();
            new Examples.ValidationHardware();
            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
