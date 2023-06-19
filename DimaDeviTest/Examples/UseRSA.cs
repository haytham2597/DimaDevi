﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimaDevi;
using DimaDevi.Formatters;
using DimaDevi.Libs;

namespace DimaDeviTest.Examples
{
    public class UseRSA
    {
        
        public UseRSA()
        {
            RSAForm rsa = new RSAForm(3072); //Generate RSA 4096
            using (DeviBuild devi = new DeviBuild(rsa).AddDisk().AddGPU(Enumerations.GPU.All))
            {
                var encrypted = devi.ToString("#");
                Console.WriteLine($"Encrypted: {encrypted}");
                var decrypted = rsa.Decrypt(encrypted);
                Console.WriteLine($"Decrypted: {decrypted}");
            }
        }
    }
}