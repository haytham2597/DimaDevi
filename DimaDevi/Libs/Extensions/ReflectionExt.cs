using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using DimaDevi.Modules;

namespace DimaDevi.Libs.Extensions
{
    public static class ReflectionExt
    {
        public static Random rnd = new Random();
        public static void SetMembers(this MemberInfo member, object obj, object value)
        {
            if (member == null)
                return;
            if(member is FieldInfo fi)
                fi.SetValue(obj, value);
            if (member is PropertyInfo pi)
                pi.SetValue(obj, value);
        }

        public static bool EqualsObject(this object a, object b)
        {
            var ma = a.GetMembers().ToList();
            var mb = b.GetMembers().ToList();
            if(ma.Count != mb.Count)
                return false;
            for (int i = 0; i < ma.Count; i++)
                if (ma[i].GetValue(a) != mb[i].GetValue(b))
                    return false;
            return true;
        }

        public static object GetValue(this MemberInfo mi, object obj)
        {
            if (mi is FieldInfo fi)
                return fi.GetValue(obj);
            if (mi is PropertyInfo pi)
                return pi.GetValue(obj);
            return null;
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

        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
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
                        char[] ch = CommonExt.RandomString(rnd.Next(4, 32)).ToCharArray();
                        for (int n = 0; n < ch.Length; n++)
                            ra.Invoke(obj, new object[] { ch[n] });
                    }
                }
                if (fields[i].FieldType == typeof(string))
                    fields[i].SetValue(obj, CommonExt.RandomString(rnd.Next(4, 32)));
                if (fields[i].FieldType == typeof(byte[]))
                    fields[i].SetValue(obj, CommonExt.GenerateRandomSalt());
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
                        char[] ch = CommonExt.RandomString(rnd.Next(4, 32)).ToCharArray();
                        for (int n = 0; n < ch.Length; n++)
                            ra.Invoke(obj, new object[] { ch[n] });
                    }
                }
                if (props[i].PropertyType == typeof(string))
                    props[i].SetValue(obj, CommonExt.RandomString(rnd.Next(4, 32)));
                if (props[i].PropertyType == typeof(byte[]))
                    props[i].SetValue(obj, CommonExt.GenerateRandomSalt());
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
    }
}
