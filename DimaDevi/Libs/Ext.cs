using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using DimaDevi.Modules;
using Newtonsoft.Json.Linq;

namespace DimaDevi.Libs
{
    public static class Ext
    {
        public static Random rnd = new Random();
        //[Obfuscation(ApplyToMembers = true, Feature = "all")]
        public static IEnumerable<Enum> GetFlags(this Enum e)
        {
            return Enum.GetValues(e.GetType()).Cast<Enum>().Where(e.HasFlag);
        }

        /// <summary>
        /// Get all Properties and Fields
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="binding"></param>
        /// <returns></returns>
        public static IEnumerable<MemberInfo> GetMembers(this object obj, BindingFlags binding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var type = obj.GetType();
            return type.GetFields(binding).Cast<MemberInfo>().Concat(type.GetProperties(binding)).Cast<MemberInfo>();
        }

        public static void SetMembers(this MemberInfo member, object obj, object value)
        {
            if (member == null)
                return;
            if(member is FieldInfo fi)
                fi.SetValue(obj, value);
            if (member is PropertyInfo pi)
                pi.SetValue(obj, value);
        }
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
                if (seenKeys.Add(keySelector(element)))
                    yield return element;
        }
        public static ElipticCurveDiffieHellman AddPublic(this ElipticCurveDiffieHellman ecdh, ECDiffieHellmanPublicKey ecdh_publickey)
        {
            ecdh.SetDerivate(ecdh_publickey);
            return ecdh;
        }

        /// <summary>
        /// Add public key of other 
        /// </summary>
        /// <param name="ecdh"></param>
        /// <param name="ecdh1"></param>
        /// <returns></returns>
        public static ElipticCurveDiffieHellman AddPublic(this ElipticCurveDiffieHellman ecdh, ElipticCurveDiffieHellman ecdh1)
        {
            return AddPublic(ecdh, ecdh1.GetPublicKey());
        }
        public static bool IsBase64(this string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }
        public static bool IsBase32(this string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[A-Z2-7\+/]*={0,3}$", RegexOptions.None);
        }
        public static bool IsHexadecimal(this string test)
        {
            return Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }
        public static byte[] DecodeHexadecimal(this string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
        public static bool IsEqual<T>(this T[] a, T[] b) where T : struct
        {
            if (a.Length != b.Length)
                return false;
            for(int i=0;i<a.Length;i++)
                if ((object)a[i] != (object)b[i])
                    return false;
            return true;
        }
        public static bool IsEqual(this byte[] f, byte[] s)
        {
            if (f.Length != s.Length)
                return false;
            for(int i=0;i<f.Length;i++)
                if (f[i] != s[i])
                    return false;
            return true;
        }
        
        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }

        [Obfuscation(Feature = "all")]
        public static bool IsEqualPropertyOrFields(this JObject jo, Type type)
        {
            return jo.IsEqualProperty(type) || jo.IsEqualFields(type);
        }

        [Obfuscation(Feature = "all")]
        public static bool IsEqualProperty(this JObject jo, Type type)
        {
            var props = type.GetProperties();
            if (props.Length != jo.Count)
                return false;
            for (int i = 0; i < props.Length; i++)
                if(!jo.ContainsKey(props[i].Name))
                    return false;
            return true;
        }
        [Obfuscation(Feature = "all")]
        public static bool IsEqualFields(this JObject jo, Type type)
        {
            var fields = type.GetFields();
            if (fields.Length != jo.Count)
                return false;
            for (int i = 0; i < fields.Length; i++)
                if (!jo.ContainsKey(fields[i].Name))
                    return false;
            return true;
        }
        public static byte[] Combine(this byte[] first, byte[] second)
        {
            byte[] bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }
        public static bool IsGenericList(this Type o)
        {
            return (o.IsGenericType && (o.GetGenericTypeDefinition() == typeof(List<>)));
        }

        public static bool IsGenericList(this Type o, Type withThis)
        {
            return (o.IsGenericType && (o.GetGenericTypeDefinition() == withThis));
        }
     
        public static bool IsPossibleIP(this string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return false;
            var reg = new Regex(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$");
            return reg.IsMatch(ip);
        }
        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];
            int cnt;
            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
                dest.Write(bytes, 0, cnt);
        }

        /// <summary>
        /// Example: Domain return \\Domain
        /// And \\Domain return \\Domain
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AddTwoBackSlashIfIsPossible(this string str)
        {
            return str.StartsWith(@"\\") ? str : @"\\" + str;
        }
        public static string Joined(this IEnumerable<IDeviComponent> components, bool prevent_duplicate = false)
        {
            if (components == null)
                throw new ArgumentNullException(nameof(components));
            if(prevent_duplicate)
                components = components.DistinctBy(x => x.BaseHardware);
            return string.Join(",", components.OrderBy(x => x.Name).Select(x => x.GetValue()));
        }
        public static byte[] GenerateRandomSalt(int size = 32)
        {
            byte[] data = new byte[size];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                for (int i = 0; i < 10; i++)
                    rng.GetBytes(data);
            return data;
        }

        public static byte[] ToMD5(this string str)
        {
            return MD5.Create().ComputeHash(GeneralConfigs.GetInstance().Encoding.GetBytes(str));
        }
        public static string ToMD5Base64(this string str)
        {
            return Convert.ToBase64String(str.ToMD5());
        }
        
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

        public static string GetMainPhysicalDriveOS()
        {
            ManagementScope scope = new ManagementScope(@"root\cimv2");
            SelectQuery query = new SelectQuery($"SELECT Antecedent,Dependent FROM Win32_LogicalDiskToPartition");
            string cap = string.Empty;
            var letter = new DriveInfo(Environment.SystemDirectory).RootDirectory.FullName.Substring(0, 2);
            string diskPart = string.Empty;
            string physicaldrive = string.Empty;
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                foreach (var mo in collection)
                {
                    if (mo["Dependent"].ToString().Contains($"DeviceID=\"{letter}\""))
                    {
                        var ante = mo["Antecedent"].ToString();
                        diskPart = ante.Split('=').Last().Replace("\"", "");
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(diskPart))
                return null;
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, new SelectQuery("SELECT Antecedent,Dependent FROM Win32_DiskDriveToDiskPartition")))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                foreach (var mo in collection)
                {
                    if (mo["Dependent"].ToString().Contains($"DeviceID=\"{diskPart}\""))
                    {
                        var ante = mo["Antecedent"].ToString();
                        return ante.Split('=').Last().Replace("\"", "").Replace("\\", "").Replace(".", "");
                    }
                }
            }
            return physicaldrive;
        }
        /// <summary>
        /// Randomizer all string, bytes[] or SecureString via Reflection for secure purpose
        /// </summary>
        /// <param name="obj"></param>
        [Obfuscation(Feature = "all")]
        public static void RandomizedStringDispose(this object obj)
        {
            var type = obj.GetType();
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var props = type.GetProperties();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].FieldType == typeof(SecureString))
                {
                    var clear = fields[i].FieldType.GetMethod("Clear");
                    if (clear != null)
                        clear.Invoke(obj, null);
                    var ra = fields[i].FieldType.GetMethod("AppendChar");
                    if (ra != null)
                    {
                        char[] ch = RandomString(rnd.Next(4, 32)).ToCharArray();
                        for (int n = 0; n < ch.Length; n++)
                            ra.Invoke(obj, new object[] { ch[n] });
                    }
                }
                if (fields[i].FieldType == typeof(string))
                    fields[i].SetValue(obj, RandomString(rnd.Next(4, 32)));
                if (fields[i].FieldType == typeof(byte[]))
                    fields[i].SetValue(obj, GenerateRandomSalt());
            }

            for (int i = 0; i < props.Length; i++)
            {
                if (props[i].PropertyType == typeof(SecureString))
                {
                    var clear = props[i].PropertyType.GetMethod("Clear");
                    if (clear != null)
                        clear.Invoke(obj, null);
                    var ra = props[i].PropertyType.GetMethod("AppendChar");
                    if (ra != null)
                    {
                        char[] ch = RandomString(rnd.Next(4, 32)).ToCharArray();
                        for (int n = 0; n < ch.Length; n++)
                            ra.Invoke(obj, new object[] { ch[n] });
                    }
                }
                if (props[i].PropertyType == typeof(string))
                    props[i].SetValue(obj, RandomString(rnd.Next(4, 32)));
                if (props[i].PropertyType == typeof(byte[]))
                    props[i].SetValue(obj, GenerateRandomSalt());
            }
        }

        /// <summary>
        /// Call disposed method if exists
        /// </summary>
        /// <param name="obj"></param>
        public static void CallDisposed(this object obj)
        {
            //Check if obj is a generic list or set
            DefaultSet.GetInstance().SetThis(obj);
            var dispose = obj.GetType().GetMethod("Dispose");
            if (dispose != null)
                dispose.Invoke(obj, null); //The normal pattern Dispose not have parameter
        }

        /// <summary>
        /// AA:BB:CC... format
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FormatMacAddress(this string input)
        {
            if (input.Length != 12 && input.Length != 16)
                return input;
            var partSize = 2;
            return string.Join(":", Enumerable.Range(0, input.Length / partSize).Select(x => input.Substring(x * partSize, partSize)));
        }
        public static string GetPublicKeyTokenFromAssembly(this Assembly assembly)
        {
            var bytes = assembly.GetName().GetPublicKeyToken();
            if (bytes == null || bytes.Length == 0)
                return "null";
            var publicKeyToken = string.Empty;
            for (int i = 0; i < bytes.GetLength(0); i++)
                publicKeyToken += $"{bytes[i]:x2}";
            return publicKeyToken;
        }

        public static string RemoveDeclarationXml(this string Content)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Content);
            foreach (XmlNode node in doc)
                if (node.NodeType == XmlNodeType.XmlDeclaration)
                    doc.RemoveChild(node);
            return doc.OuterXml;
        }

        public static byte[] EncodeToByte(string text)
        {
            var bytes = new byte[Encoding.UTF8.GetByteCount(text) + 1];
            object syncLock = new object();
            lock (syncLock)
                rnd.NextBytes(bytes);

            Encoding.UTF8.GetBytes(text, 0, text.Length, bytes, 1);

            unchecked
            {
                for (int i = 1; i < bytes.Length; ++i)
                    bytes[i] += bytes[0];
            }

            var base64String = Convert.ToBase64String(bytes);
            byte[] newBytes = Encoding.ASCII.GetBytes(base64String);
            return newBytes;
        }
        public static string DecodeToByte(this byte[] bytes)
        {
            string someString = Encoding.ASCII.GetString(bytes);
            byte[] data = Convert.FromBase64String(someString);

            unchecked
            {
                for (int i = 1; i < data.Length; ++i)
                    data[i] -= data[0];
            }

            data = data.Skip(1).ToArray();
            string decodedString = Encoding.UTF8.GetString(data);
            return decodedString;
        }
        /// <summary>
        /// Get a Dictionary of Fullname from Assembly (that contain FullName, Version, Culture and PublicKeyToken)
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetFullNameDictFromAssembly(this Assembly assembly)
        {
            Dictionary<string, string> dictFullName = new Dictionary<string, string>();
            var spl =assembly.FullName.Split(',');
            var spl1 = spl[0].Split(':');
            dictFullName.Add("FullName", spl1.LastOrDefault()?.Trim());
            dictFullName.Add("Version", spl[1].Split('=').LastOrDefault()?.Trim());
            dictFullName.Add("Culture", spl[2].Split('=').LastOrDefault()?.Trim());
            dictFullName.Add("PublicKeyToken", spl[3].Split('=').LastOrDefault()?.Trim());
            return dictFullName;
        }
    }
}
