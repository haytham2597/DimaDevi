using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using DimaDevi.Libs;

namespace DimaDevi.Formatters
{
    public sealed class HashForm : IDeviFormatter
    {
        /// <summary>
        /// A function that returns the hash algorithm to use.
        /// </summary>
        private readonly Func<HashAlgorithm> _hashAlgorithm;

        /// <summary>
        /// Default MD5
        /// </summary>
        public HashForm()
        {
            _hashAlgorithm = MD5.Create;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="HashForm"/> class.
        /// </summary>
        /// <param name="hashAlgorithm">A function that returns the hash algorithm to use.</param>
        public HashForm(Func<HashAlgorithm> hashAlgorithm)
        {
            _hashAlgorithm = hashAlgorithm ?? throw new ArgumentNullException(nameof(hashAlgorithm)); //Can not be null
        }
        
        /// <summary>
        /// Returns the device identifier string created by combining the specified <see cref="IDeviComponent"/> instances.
        /// </summary>
        /// <param name="components">A sequence containing the <see cref="IDeviComponent"/> instances to combine into the device identifier string.</param>
        /// <returns>The device identifier string.</returns>
        public string GetDevi(IEnumerable<IDeviComponent> components)
        {
            return Hash(components.Joined(GeneralConfigs.GetInstance().PreventDuplicationComponents));
        }

        public string GetDevi(string componentsResult, string separator)
        {
            return Hash(componentsResult);
        }

        public string Hash(string res)
        {
            var bytes = GeneralConfigs.GetInstance().Encoding.GetBytes(res);
            var algorithm = _hashAlgorithm.Invoke();
            var hash = algorithm.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public void Dispose()
        {

        }
    }
}
