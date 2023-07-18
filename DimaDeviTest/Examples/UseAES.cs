using System;
using System.Security.Cryptography;
using DimaDevi;
using DimaDevi.Formatters;

namespace DimaDeviTest.Examples
{
    public class UseAES
    {
        public UseAES()
        {
            using (var devi = new DeviBuild().AddMotherboard().AddMachineUUID())
            {
                devi.Formatter = new AESForm("TheCoolPassword"){Cipher = CipherMode.OFB};
                /*var salt = (devi.Formatter as AESForm).GetSalt(); //This is just show how get salt
                (devi.Formatter as AESForm).Salt = salt; //This is just for show you how SET salt or modified*/
                string content = devi.ToString();
                string decryption = devi.DecryptionDecode(content); //In different context u should set salt before decryption
                Console.WriteLine($"Content: {content}");
                Console.WriteLine($"Decryption: {decryption}");
            }
        }
    }
}
