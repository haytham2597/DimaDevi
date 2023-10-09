using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using DimaDevi.Components;
using DimaDevi.Formatters;
using DimaDevi.Hardware;
using DimaDevi.Libs;
using DimaDevi.Libs.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace DimaDevi
{
    public class DeviBuild : IDisposable
    {
        //TODO: Automatically detect and support Linux
        /*private class ObservableComponent<IDeviComponent> : ObservableCollection<IDeviComponent>
        {

        }*/
        public ObservableCollection<IDeviComponent> Components;
        //public IList<IDeviComponent> Components;
        public IDeviFormatter Formatter;
        public RemoteWMICredential WmiCredential
        {
            set => DeviGeneralConfig.GetInstance().RemoteWmi = value;
        }

        /// <summary>
        /// Clear components after call ToString
        /// </summary>
        public bool ClearAfterProcess;

        /// <summary>
        /// Make all Distinct components
        /// </summary>
        public bool PreventComponentDuplication
        {
            set => DeviGeneralConfig.GetInstance().PreventDuplicationComponents = value;
        }

        public DeviBuild()
        {
            
            Components = new ObservableCollection<IDeviComponent>();
            Components.CollectionChanged += Components_CollectionChanged;
            //Components = new List<IDeviComponent>();
            //Load user-defined components
            var hard = HardwareComponents.GetInstance().GetHardware();
            for (int i = 0; i < hard.Count; i++)
            {
                var elem = hard.ElementAt(i);
                var last = elem.Key.Name;
                if (string.IsNullOrEmpty(last) || !Dict.WMIClass.ContainsKey(last))
                    continue;
                for (int j = 0; j < elem.Value.Count; j++)
                    Components.Add(new WMIComp(elem.Value[j], Dict.WMIClass[last], elem.Value[j]) { BaseHardware = last });
            }
        }

        private void Components_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;
            if (!DeviGeneralConfig.GetInstance().AllowSingletonComponents)
                return;
            var dii = DeviInstanceInvocation.GetInstance();
            if (sender is IList<IDeviComponent> deviComp)
            {
                
                for (int i = 0; i < deviComp.Count; i++)
                {
                    if (!dii.Components.Contains(deviComp[i]))
                        dii.Components.Add(deviComp[i]);
                }
            }

            if (sender is IDeviComponent devi)
            {
                if(!dii.Components.Contains(devi))
                    dii.Components.Add(devi);
            }
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

        /// <summary>
        /// Save all information on the disk with formatter
        /// </summary>
        /// <param name="pathToSave">Path where save this file</param>
        /// <param name="formatter">A custom formatter instaed of formater created in class</param>
        /// <param name="separator">Separator symbols</param>
        public void Save(string pathToSave, IDeviFormatter formatter, string separator = null)
        {
            File.WriteAllText(Path.Combine(pathToSave), ToString(formatter, separator));
        }

        public virtual bool Validate(string result, string separator = null)
        {
            return DecryptionDecode(result) == DecryptionDecode(ToString(separator));
        }
        
        public virtual double Validate(Hardwares hardwares)
        {
            //WARNING: Bad validation if user-defined new components with HardwareComponents i mean, this validation don't depend on user-defined new component and should...
            var this_hard = GetHardwares();
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

            //Enumerations.ToleranceLevel tolerance;

            double percentage = (double)(cnt_match * 100) / props_hard.Length;
            return percentage;
        }

        public void ClearComponents()
        {
            Components.Clear();
        }

        /// <summary>
        /// Decrypt or Decode
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string DecryptionDecode(string content)
        {
            if (Formatter == null)
                return content;
            if (DeviGeneralConfig.GetInstance().IsObfuscated)
            {
                var methods = Formatter.GetType().GetMethods(BindingFlags.Public);
                for (int i = 0; i < methods.Length; i++)
                {
                    var attr = methods[i].GetCustomAttributes(true);
                    for (int j = 0; j < attr.Length; j++)
                        if (attr[j] is Attrs.MethodNameAttribute mna && mna.MethodName == "Decrypt")
                            return methods[i].Invoke(Formatter, new object[] { content }).ToString();
                }
            }

            var decode = Formatter.GetType().GetMethod("Decode");
            if (decode != null) //Is decoded method not decrypt
            {
                var result =  decode.Invoke(Formatter, new object[] { content });
                if (result is byte[] b)
                    return DeviGeneralConfig.GetInstance().Encoding.GetString(b);
            }

            var decrypt = Formatter.GetType().GetMethod("Decrypt");
            return decrypt != null ? decrypt.Invoke(Formatter, new object[] { content }).ToString() : content;
        }

        public string Decryption(string content, IDeviFormatter formatter)
        {
            Formatter = formatter;
            return DecryptionDecode(content);
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
                    if (props[i].Name.ToLower() != d.Key.ToLower()) //ToLower for prevent possible human error about uppercase, capitalize, etc.
                        continue;
                    var objInstance = Activator.CreateInstance(props[i].PropertyType); //Because all propertiies of Hardware is a class
                    var propsInstance = objInstance.GetType().GetProperties();
                    using (var enumer = d.GetEnumerator())
                    {
                        while (enumer.MoveNext())
                        {
                            for (int j = 0; j < propsInstance.Length; j++)
                            {
                                if (enumer.Current?.Name != nameof(MacAddress) && propsInstance[j].Name.ToLower() != enumer.Current?.Name?.ToLower()) //ToLower for prevent possible error human about uppercase, capitalize, etc.
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
            var dii = DeviInstanceInvocation.GetInstance();
            string result = Formatter != null ? Formatter.GetDevi(DeviGeneralConfig.GetInstance().AllowSingletonComponents ? dii.Components : Components) : (DeviGeneralConfig.GetInstance().AllowSingletonComponents ? dii.Components : Components).Joined(DeviGeneralConfig.GetInstance().PreventDuplicationComponents);
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
            if (!DeviGeneralConfig.GetInstance().ProcessComponentsWhileAdd)
            {
                if (DeviGeneralConfig.GetInstance().PreventDuplicationComponents)
                    enumer = Components.DistinctBy(x => x.BaseHardware).GetEnumerator();
                while (enumer.MoveNext())
                {
                    if (DeviGeneralConfig.GetInstance().ExcludeNameComponentString)
                    {
                        str += enumer.Current?.GetValue() + separator;
                    }
                    else
                    {
                        str += enumer.Current?.Name + "=" + enumer.Current?.GetValue() + separator;
                    }
                }
            }
            else
            {
                var res = DeviGeneralConfig.GetInstance().result;
                for (int i = 0; i < res.Count; i++)
                    str += res[i] + separator;
            }

            enumer.Dispose();

            if (ClearAfterProcess)
                ClearComponents();
            str = str.TrimEnd(separator.ToCharArray());
            return Formatter != null ? Formatter.GetDevi(str, separator) : str;
        }

        public string ToString(IDeviFormatter formatter, string separator =null)
        {
            Formatter = formatter;
            return ToString(separator);
        }

        public string[] ToArray(string separator = null)
        {
            List<string> obj = new List<string>();
            using (IEnumerator<IDeviComponent> enumer = Components.GetEnumerator())
            {
                while (enumer.MoveNext())
                {
                    if (DeviGeneralConfig.GetInstance().ExcludeNameComponentString)
                    {
                        obj.Add(enumer.Current?.GetValue() + separator);
                    }
                    else
                    {
                        obj.Add(enumer.Current?.Name + "=" + enumer.Current?.GetValue() + separator);
                    }
                }
            }
            return obj.ToArray();
        }

        public object[] ToArray(IDeviFormatter formatter, string separator = null)
        {
            List<object> obj = new List<object>();
            using (IEnumerator<IDeviComponent> enumer = Components.GetEnumerator())
                while (enumer.MoveNext())
                    obj.Add(DeviGeneralConfig.GetInstance().ExcludeNameComponentString ? formatter.GetDevi(enumer.Current?.GetValue(), separator) : formatter.GetDevi(enumer.Current?.Name + "=" + enumer.Current?.GetValue(), separator));
            return obj.ToArray();
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
        /// Import formatter provider by user because if you want decrypt content, you should set settings formatter or import
        /// </summary>
        /// <param name="str"></param>
        public void ImportFormatter(string str)
        {
            JObject jo = JObject.Parse(str);
            Type[] typelist = ReflectionExt.GetTypesInNamespace(Assembly.GetExecutingAssembly(), typeof(AESForm).Namespace);
            for (int i = 0; i < typelist.Length; i++)
            {
                var ctor = typelist[i].GetConstructor(new[] { typeof(JObject) });
                if (ctor == null)
                    continue;
                if(jo.IsEqualPropertyOrFields(typelist[i]))
                    Formatter = (IDeviFormatter)ctor.Invoke(new object[] { jo });
            }
        }

        /// <summary>
        /// ZIP content
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] Compression(string str)
        {
            var bytes = DeviGeneralConfig.GetInstance().Encoding.GetBytes(str);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                    msi.CopyTo(gs);
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
                    gs.CopyTo(mso);
                return DeviGeneralConfig.GetInstance().Encoding.GetString(mso.ToArray());
            }
        }

        public IDeviComponent this[string nameHardware]
        {
            get
            {
                //TODO: Add pending if not exists 
                return Components.FirstOrDefault(x => x.BaseHardware == nameHardware);
            }
        }

        public IDeviComponent this[string name, string baseHardware]
        {
            get
            {
                return Components.FirstOrDefault(x => x.Name == name && x.BaseHardware == baseHardware);
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            var cop =Components[oldIndex];
            this.Components.Remove(cop);
            this.Components.Insert(newIndex, cop);
        }
        public void Dispose()
        {
            Formatter.Dispose();
            using (var enumer = Components.GetEnumerator())
                while (enumer.MoveNext())
                    enumer.Current?.CallDisposed();
            Components.Clear();
        }
    }
}
