using System;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DimaDevi.Modules
{
    public sealed class ElipticCurveDiffieHellman : IDisposable
    {
        private ECDiffieHellmanCng ecdh;
        private CngKey CngKey;
        private byte[] privateKey;
        public ElipticCurveDiffieHellman(int key_size = 256)
        {
            CngKey = InitCngKey(key_size);
            ecdh = new ECDiffieHellmanCng(CngKey)
            {
                KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
                HashAlgorithm = CngAlgorithm.Sha256,
                KeySize = key_size
            };
        }

        /// <summary>
        /// Only 256, 384 or 521
        /// <para>160 = 1024 in RSA</para>
        /// <para>224 = 2048 in RSA</para>
        /// <para>256 = 3072 in RSA</para>
        /// <para>384 = 7680 in RSA</para>
        /// <para>521 = 15360 in RSA</para>
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="key_size"></param>
        public ElipticCurveDiffieHellman(ECDiffieHellmanPublicKey publicKey, int key_size = 256)
        {
            CngKey = InitCngKey(key_size);
            ecdh = new ECDiffieHellmanCng(CngKey)
            {
                KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
                HashAlgorithm = CngAlgorithm.Sha256,
                KeySize = key_size
            };
            privateKey = ecdh.DeriveKeyMaterial(CngKey.Import(publicKey.ToByteArray(), CngKeyBlobFormat.EccPublicBlob));
        }

        public ElipticCurveDiffieHellman(ECDiffieHellmanCng ecdhCng, int key_size = 256)
        {
            CngKey = InitCngKey(key_size);
            ecdh = ecdhCng;
        }
        public ElipticCurveDiffieHellman(ECDiffieHellmanCng ecdhCng, ECDiffieHellmanPublicKey publicKey, int key_size = 256) : this(publicKey, key_size)
        {
            CngKey = InitCngKey(key_size);
            ecdh = ecdhCng;
        }

        private CngKey InitCngKey(int key_size)
        {
            return CngKey.Create(InitAlgorithm(key_size), null, new CngKeyCreationParameters() { ExportPolicy = CngExportPolicies.AllowPlaintextExport });
        }

        private CngAlgorithm InitAlgorithm(int key_size)
        {
            CngAlgorithm cngAlgorithm = CngAlgorithm.ECDiffieHellmanP256; //default
            if (key_size == 384)
                cngAlgorithm = CngAlgorithm.ECDiffieHellmanP384;
            if (key_size == 521)
                cngAlgorithm = CngAlgorithm.ECDiffieHellmanP521;
            return cngAlgorithm;
        }

        /// <summary>
        /// Is public of this ECDH
        /// </summary>
        /// <returns></returns>
        public ECDiffieHellmanPublicKey GetPublicKey()
        {
            return ecdh.PublicKey;
        }

        /// <summary>
        /// Is Private of this ECDH, is should be used for save in disk
        /// NEVER PUBLISH THIS KEY
        /// </summary>
        /// <returns></returns>
        public byte[] GetPrivateKey()
        {
            return CngKey.Export(CngKeyBlobFormat.EccPrivateBlob);
        }
        
        /// <summary>
        /// Is Mixed of public key bob with our private and public key
        /// </summary>
        /// <returns></returns>
        public byte[] GetDerivate()
        {
            return privateKey;
        }

        public void SetDerivate(ECDiffieHellmanPublicKey ecdh_publickey)
        {
            byte[] data = ecdh_publickey.ToByteArray();
            if (data == null)
                throw new Exception("ByteArray is null");
            privateKey = ecdh.DeriveKeyMaterial(CngKey.Import(data, CngKeyBlobFormat.EccPublicBlob));
        }

        public string Export()
        {
            //https://davidtavarez.github.io/2019/implementing-elliptic-curve-diffie-hellman-c-sharp/
            //https://stackoverflow.com/questions/48522005/c-sharp-importing-a-public-key-blob-into-ecdiffiehellmancng
            //https://stackoverflow.com/questions/20505325/how-to-export-private-key-for-ecdiffiehellmancng
            JObject job = new JObject();
            job["ecdh"] = JToken.FromObject(ecdh.ExportParameters(true));
            //job["cng"] = CngKey.Export(CngKeyBlobFormat.EccFullPrivateBlob);
            return JsonConvert.SerializeObject(job, Formatting.Indented);
        }

        public void Import(string data)
        {
            var job = JObject.Parse(data);
            ecdh.ImportParameters(job["ecdh"].ToObject<ECParameters>());
            //ecdh = new ECDiffieHellmanCng(CngKey.Import(job["cng"].ToObject<byte[]>(), CngKeyBlobFormat.EccFullPrivateBlob));
            //job["cng"] = CngKey.Export(CngKeyBlobFormat.GenericPrivateBlob);
            //CngKey = CngKey.Import(job["cng"].ToObject<byte[]>(), CngKeyBlobFormat.GenericPrivateBlob);
            //this.CngKey = CngKey.Import(keyBlob, CngKeyBlobFormat.EccFullPrivateBlob);
        }

        public void Dispose()
        {
            ecdh.Dispose();
        }
    }
}
