using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using DimaDevi.Formatters;
using DimaDevi.Hardware;
using DimaDevi.Libs;
using DimaDevi.Modules;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DimaDevi
{
    public class DeviBuild : IDisposable
    {
        public ISet<IDeviComponent> Components { set; get; }
        public IDeviFormatter Formatter { set; get; }
        public Property.RemoteWMICredential WmiCredential { set; get; }
        public bool IsObfuscated { set; get; }
        public bool ClearAfterProcess { set; get; }

        private bool preventComponentDuplication;
        public bool PreventComponentDuplication
        {
            set
            {
                preventComponentDuplication = value;
                this.Formatter.PreventComponentDuplication = value;
            }
            get => preventComponentDuplication;
        }

        public DeviBuild()
        {
            Components = new HashSet<IDeviComponent>();
            this.IsObfuscated = nameof(DeviBuild) == "DeviBuild";
        }

        public DeviBuild(IDeviFormatter formatter) : this()
        {
            Formatter = formatter;
        }
       
        /// <summary>
        /// Save all information on the disk
        /// </summary>
        /// <param name="pathToSave"></param>
        /// <param name="separator"></param>
        public void Save(string pathToSave, string separator = null)
        {
            File.WriteAllText(Path.Combine(pathToSave), ToString(separator));
        }

        public void Save(string pathToSave, IDeviFormatter formatter, string separator = null)
        {
            File.WriteAllText(Path.Combine(pathToSave), ToString(formatter, separator));
        }

        public virtual bool Validate(string result, string separator = null)
        {
            return Decryption(result) == Decryption(ToString(separator));
        }

        public virtual double Validate(Hardwares hardwares)
        {
            var this_hard = this.GetHardwares();
            var props_hard = this_hard.GetType().GetProperties();
            var props_hardwares = hardwares.GetType().GetProperties();
            if (props_hard.Length != props_hardwares.Length)
                return 0;

            int cnt_unmatch = 0;
            int cnt_match=0;
            for (int i = 0; i < props_hard.Length; i++)
            {
                if (!props_hard[i].Equals(props_hardwares[i]))
                {
                    cnt_unmatch++;
                    continue;
                }
                cnt_match++;
            }

            double percentage = (double)(cnt_match * 100) / props_hard.Length;
            return percentage;
        }

        public void ClearComponents()
        {
            this.Components.Clear();
        }

        public string Decryption(string content)
        {
            if (Formatter == null)
                return content;
            var decrypt = Formatter.GetType().GetMethod("Decrypt");
            return decrypt != null ? decrypt.Invoke(Formatter, new object[] { content }).ToString() : content;
        }

        public string Decryption(string content, IDeviFormatter formatter)
        {
            this.Formatter = formatter;
            return Decryption(content);
        }

        [Obfuscation(Feature = "all")]
        public Hardwares GetHardwares()
        {
            if (Components == null)
                throw new Exception($"Null {nameof(Components)}");
            Hardwares hardwares = new Hardwares();
            var props = hardwares.GetType().GetProperties();
            var gro = Components.GroupBy(x => x.BaseHardware);
            for (int i = 0; i < props.Length; i++)
            {
                foreach (var d in gro)
                {
                    if (string.IsNullOrEmpty(d.Key))
                        continue;
                    if (props[i].Name.ToLower() != d.Key.ToLower()) //ToLower for prevent possible error human about uppercase, capitalize, etc.
                        continue;
                    var objInstance = Activator.CreateInstance(props[i].PropertyType); //Because all propertiies of Hardware is a class
                    var propsInstance = objInstance.GetType().GetProperties();
                    using (var enumer = d.GetEnumerator())
                    {
                        while (enumer.MoveNext())
                        {
                            for (int j = 0; j < propsInstance.Length; j++)
                            {
                                if (enumer.Current?.Name != nameof(MacAddress) && propsInstance[j].Name.ToLower() != enumer.Current?.Name.ToLower()) //ToLower for prevent possible error human about uppercase, capitalize, etc.
                                    continue;

                                var result = enumer.Current.GetValue();
                                if (result.Contains(',') && propsInstance[j].PropertyType.IsGenericList())
                                {
                                    var spl = result.Split(',').ToList();
                                    spl.RemoveAll(string.IsNullOrEmpty);
                                    var propTypeGen = propsInstance[j].PropertyType.GetGenericArguments()[0];
                                    IList res = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(propTypeGen));
                                    for(int n=0;n< spl.Count;n++)
                                        res.Add(Convert.ChangeType(spl[n], propTypeGen));
                                    propsInstance[j].SetValue(objInstance, res);
                                    continue;
                                }
                                propsInstance[j].SetValue(objInstance, Convert.ChangeType(result, propsInstance[j].PropertyType));
                            }
                        }
                    }
                    props[i].SetValue(hardwares, objInstance);
                }
            }
            return hardwares;
        }

        public override string ToString()
        {
            string result = Formatter != null ? Formatter.GetDevi(Components) : Components.Joined(this.PreventComponentDuplication);
            if (ClearAfterProcess)
                ClearComponents();
            return result;
        }

        public string ToString(string separator)
        {
            if (string.IsNullOrEmpty(separator))
                return ToString();
            string str = string.Empty;

            IEnumerator<IDeviComponent> enumer = Components.GetEnumerator();
            if (PreventComponentDuplication)
                enumer = Components.DistinctBy(x => x.BaseHardware).GetEnumerator();
            while (enumer.MoveNext())
                str += enumer.Current?.Name + "=" + enumer.Current?.GetValue() + separator;

            enumer.Dispose();

            if (ClearAfterProcess)
                ClearComponents();
            return Formatter != null ? Formatter.GetDevi(str, separator) : str.TrimEnd(separator.ToCharArray());
        }

        public string ToString(IDeviFormatter formatter, string separator =null)
        {
            this.Formatter = formatter;
            return ToString(separator);
        }

        public IEnumerable<IGrouping<string, IDeviComponent>> ToGroup()
        {
            return Components.GroupBy(x => x.BaseHardware);
        }
        /// <summary>
        /// Get specific components that was added
        /// <para>Only get the string of the component that was added in <see cref="DeviBuild"/></para>
        /// </summary>
        /// <param name="enume"></param>
        /// <returns></returns>
        public string GetSpecificComponent(Enum enume)
        {
            return Components.FirstOrDefault(x => x.BaseHardware.ToLower() == enume.GetType().Name.ToLower() && x.Name.ToLower() == enume.ToString().ToLower())?.GetValue();
        }

        /// <summary>
        /// Very useful if you used RSA or AES as formatter. Public property is revealed
        /// </summary>
        /// <returns></returns>
        public string GetInfoFormatter()
        {
            return Formatter == null ? null : JsonConvert.SerializeObject(Formatter);
        }

        /// <summary>
        /// Import formatter provider user because if you want decrypt content, you should set settings formatter or import
        /// </summary>
        /// <param name="str"></param>
        public void ImportFormatter(string str)
        {
            JObject jo = JObject.Parse(str);
            Type[] typelist = Ext.GetTypesInNamespace(Assembly.GetExecutingAssembly(), typeof(AESForm).Namespace);
            for (int i = 0; i < typelist.Length; i++)
            {
                var ctor = typelist[i].GetConstructor(new[] { typeof(JObject) });
                if (ctor == null)
                    continue;
                if(jo.IsEqualPropertyOrFields(typelist[i]))
                    this.Formatter = (IDeviFormatter)ctor.Invoke(new object[] { jo });
            }
        }

        /// <summary>
        /// ZIP content
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] Compression(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                    Ext.CopyTo(msi, gs);
                return mso.ToArray();
            }
        }
        /// <summary>
        /// UnZIP content
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string Descompression(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    Ext.CopyTo(gs, mso);
                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        public void Dispose()
        {
            Formatter.Dispose();
            Components.Clear();
            this.WmiCredential.Dispose();
        }
    }
}
