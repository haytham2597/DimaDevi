using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using DimaDevi.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DimaDevi.Libs.Extensions
{
    public static class CommonExt
    {
        public static Random rnd = new Random();
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
        public static Type GetObjType(object m)
        {
            if (!(m is MemberInfo mi)) 
                return m.GetType();
            
            if (mi is FieldInfo fi)
                return fi.FieldType;
            else if (mi is PropertyInfo pi)
                return pi.PropertyType;
            else
                return m.GetType().BaseType;
        }
        public static bool IsCollectionType(this Type type)
        {
            if (!type.GetGenericArguments().Any())
                return false;

            Type genericTypeDefinition = type.GetGenericTypeDefinition();
            var collectionTypes = new[] { typeof(IEnumerable<>), typeof(ICollection<>), typeof(IList<>), typeof(List<>), typeof(IList) };
            return collectionTypes.Any(x => x.IsAssignableFrom(genericTypeDefinition));
        }
        public static bool IsStandardType(this object obj)
        {
            try
            {
                Type t = GetObjType(obj);
                if (t.IsGenericType)
                {
                    if (t.GetGenericTypeDefinition() == typeof(Func<>) || t.GetGenericTypeDefinition() == typeof(Func<,>))
                        return false;
                }

                if (!IsCollectionType(t) || t.GetGenericArguments().Length == 0)
                    return t.Namespace != null && t.Namespace.StartsWith("System") && t.Module.ScopeName == "CommonLanguageRuntimeLibrary";

                var generic_args = t.GetGenericArguments();
                for (int i = 0; i < generic_args.Length; i++)
                    if (!IsStandardType(generic_args[i]))
                        return false;
                return t.Namespace != null && t.Namespace.StartsWith("System") && t.Module.ScopeName == "CommonLanguageRuntimeLibrary";
            }
            catch
            {
                return false;
            }
        }
        public static bool IsJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            return false;
        }

        public static byte[] DecodeHexadecimal(this string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return raw;
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
            return MD5.Create().ComputeHash(DeviGeneralConfig.GetInstance().Encoding.GetBytes(str));
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

        public static string RemoveDeclarationXml(this string Content)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Content);
            foreach (XmlNode node in doc)
                if (node.NodeType == XmlNodeType.XmlDeclaration)
                    doc.RemoveChild(node);
            return doc.OuterXml;
        }

        public static byte[] ToByte(this string text)
        {
            return DeviGeneralConfig.GetInstance().Encoding.GetBytes(text);
        }

        public static object[] ToBytesArrayEncode(this string[] text)
        {
            var obj =new object[text.Length];
            for (int i = 0; i < text.Length; i++)
                obj[i] = DeviGeneralConfig.GetInstance().Encoding.GetBytes(text[i]);
            return obj;
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

        public static bool IsHexadecimal(this string text)
        {
            return Regex.IsMatch(text, @"\A\b[0-9a-fA-F]+\b\Z");
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
