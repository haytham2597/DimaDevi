using System;
using DimaDevi;
using DimaDevi.Formatters;
using DimaDevi.Libs;
using DimaDevi.Libs.Extensions;
using DimaDevi.Modules;

namespace DimaDeviTest.Examples
{
    /// <summary>
    /// ECDH with AES
    /// </summary>
    public class ECDHAES
    {
        public ECDHAES()
        {
            DeviBuild devi = new DeviBuild()
                .AddCPU(Enumerations.CPU.All)
                .AddMachineName()
                .AddMacAddress()
                .AddMotherboard()
                .AddUUID()
                .AddRam(Enumerations.RAM.All)
                .AddRegistry(@"SOFTWARE\\DefaultUserEnvironment", "Path");

            var bob = new ElipticCurveDiffieHellman(); //Instance ECDH [Bob]
            var alice = new ElipticCurveDiffieHellman(bob.GetPublicKey()); //Instance ECDH [Alice] with public key of Bob. Alice have public key too.

            Func<string> deviString = () => devi.ToString("<MyCoolCustomSeparator>");

            var aesAlice = new AESForm(alice); //What happen with salt? Salt is a Derivate of ECDH
            Console.WriteLine($"Original Data: {deviString()}");
            Console.WriteLine("-----------");
            devi.Formatter = aesAlice; //Set AES Formatter

            var IVAlice = aesAlice.GetIV(); //Get IV Vector [Alice]
            string content = deviString(); //
            var aesBob = new AESForm(bob.AddPublic(alice)); //in ECDH of [Bob] add Public key of [Alice]
            
            aesBob.SetIV(IVAlice); //Set IV vector of [Alice] in AES [Bob]
            Console.WriteLine($"Cipher Data: {content}");
            Console.WriteLine("-----------");
            var decrypt = devi.Decryption(content, aesBob);
            Console.WriteLine($"Decrypt data: {decrypt}");
        }
    }
}
