using System;
using DimaDevi;
using DimaDevi.Formatters;
using DimaDevi.Libs;
using DimaDevi.Modules;

namespace DimaDeviTest.Examples
{
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

            var bob = new ElipticCurveDiffieHellman(); //Instance ECDH Bob
            var alice = new ElipticCurveDiffieHellman(bob.GetPublicKey()); //Instance ECDH alice with public key of Bob. Alice have public key too.

            Func<string> deviString = () => devi.ToString("<MyCoolCustomSeparator>");

            var aesAlice = new AESForm(alice); //What happen with salt? Salt is a Derivate of ECDH
            Console.WriteLine($"Original Data: {deviString()}");
            Console.WriteLine("-----------");
            devi.Formatter = aesAlice; //Set AES Formatter

            var IVAlice = aesAlice.GetIV(); //Get IV Vector Alice
            string content = deviString(); //
            var aesBob = new AESForm(bob.AddPublic(alice)); //in ECDH of BOB add Public key of Alice
            
            aesBob.SetIV(IVAlice); //Set IV vector of Alice in AES Bob
            Console.WriteLine($"Cipher Data: {content}");
            Console.WriteLine("-----------");
            var decrypt = devi.Decryption(content, aesBob);
            Console.WriteLine($"Decrypt data: {decrypt}");
            var f = devi.GetInfoFormatter();
            /*Console.WriteLine("Prev form :"+devi.Formatter.GetType().FullName);
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
        }
    }
}
