using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimaDevi;
using DimaDevi.Libs;

namespace DimaDeviTest.Examples
{
    internal class CompressAndDescompress
    {
        public CompressAndDescompress()
        {
            using (var devi = new DimaDevi.DeviBuild().AddCPU(Enumerations.CPU.All).AddMotherboard(Enumerations.Motherboard.Manufacturer | Enumerations.Motherboard.Product).AddRam(Enumerations.RAM.All))
            {
                var original = devi.ToString();
                Console.WriteLine($"Bytes length original: {DeviGeneralConfig.GetInstance().Encoding.GetBytes(original).Length}");
                var compress = devi.Compression(devi.ToString()); 
                Console.WriteLine($"Bytes length compression: {compress.Length}");
                Console.WriteLine("--++--");
                Console.WriteLine(original);
                Console.WriteLine("----");
                Console.WriteLine(devi.Descompression(compress));
            }
        }
    }
}
