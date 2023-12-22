using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using DimaDevi.Libs;
using DimaDevi.Libs.Extensions;

namespace DimaDevi.Components
{
    /// <summary>
    /// An implementation of <see cref="IDeviComponent"/> that retrieves data from a WMI class.
    /// </summary>
    public sealed class WMIComp : IDeviComponent
    {
        /// <summary>
        /// For fast response
        /// </summary>
        private string Result = string.Empty;

        public Func<string, string> Replacement { get; set; }
        public string BaseHardware { set; get; } = null;
        public string Name { get; }
        /// <summary>
        /// The WMI class name. Example Win32_Processor
        /// </summary>
        private readonly string _wmiClass;

        /// <summary>
        /// The WMI property name. Example: Manufacturer
        /// </summary>
        private readonly string _wmiProperty;
        private readonly string _wmiName;
        private readonly string _wmiWhere;

        /// <summary>
        /// Initializes a new instance of the <see cref="WMIComp"/> class.
        /// SELECT wmiProperty FROM wmiClass
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <param name="wmiClass">The WMI class name.</param>
        /// <param name="wmiProperty">The WMI property name. Support comma "Caption, Description"</param>
        public WMIComp(string name, string wmiClass, string wmiProperty)
        {
            Name = name;
            _wmiClass = wmiClass;
            _wmiProperty = wmiProperty;
        }
        public WMIComp(string name, string wmiClass, string wmiProperty, string wmiName) : this(name, wmiClass, wmiProperty)
        {
            _wmiName = wmiName;
        }

        public WMIComp(string name, string wmiClass, string wmiProperty, string wmiName, string wmiWhere) : this(name, wmiClass, wmiProperty,wmiName)
        {
            _wmiWhere = wmiWhere;
        }

        /// <summary>
        /// Gets the component value.
        /// </summary>
        /// <returns>The component value.</returns>
        public string GetValue()
        {
            if (!string.IsNullOrEmpty(Result))
                return Result;
            //TODO: Prevent or include 2 CPU
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(_wmiClass) || string.IsNullOrEmpty(_wmiProperty))
                return null;

            var values = new List<string>();
            try
            {
                ConnectionOptions connectOptions = new ConnectionOptions();
                ManagementScope scope = new ManagementScope(@"root\cimv2");
                if (DeviGeneralConfig.GetInstance().RemoteWmi != null && !DeviGeneralConfig.GetInstance().RemoteWmi.IsEmpty())
                {
                    connectOptions.Username = DeviGeneralConfig.GetInstance().RemoteWmi.Username;
                    connectOptions.Password = DeviGeneralConfig.GetInstance().RemoteWmi.Password;
                    scope.Path = new ManagementPath(DeviGeneralConfig.GetInstance().RemoteWmi.Domain.AddTwoBackSlashIfIsPossible() + @"\root\cimv2");
                    scope.Options = connectOptions;
                }
                SelectQuery query = new SelectQuery($"SELECT {_wmiProperty} FROM {_wmiClass}");
                if (!string.IsNullOrEmpty(_wmiWhere))
                    query.QueryString += $" WHERE {_wmiWhere}";
                IList<string> splitProp = null;
                if (_wmiProperty.Contains(","))
                    splitProp =_wmiProperty.Split(',').Select(x => x.Replace(" ", "")).ToList();
                
                using (ManagementObjectSearcher searcher =new ManagementObjectSearcher(scope, query))
                using (ManagementObjectCollection collection = searcher.Get())
                {
                    foreach (var mo in collection)
                    {
                        if (!string.IsNullOrEmpty(_wmiWhere))
                        {
                            if (splitProp != null)
                            {
                                for (int i = 0; i < splitProp.Count; i++)
                                    values.Add(mo[splitProp[i]].ToString().Trim());
                                break;
                            }

                            var spl = _wmiWhere.Split('=');
                            if (mo[spl[0]].ToString().Contains(spl[1]))
                            {
                                values.Add(mo[_wmiProperty == "*" ? _wmiName : _wmiProperty].ToString().Trim());
                                break;
                            }
                            continue;
                        }

                        /*if (_wmiProperty == "*")
                        {
                            foreach (var prorpo in mo.Properties)
                            {
                                var g = mo[prorpo.Name];
                                if (g != null)
                                {
                                    values.Add(g.ToString().Trim());
                                }
                                
                            }
                        }*/
                        
                        values.Add(mo[_wmiProperty == "*" ? _wmiName : _wmiProperty].ToString().Trim());
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            values.Sort();
            if (Replacement != null)
            {
                Result = Replacement((values.Count > 0) ? string.Join(",", values) : null);
                return Result;
            }
            Result = (values.Count > 0) ? string.Join(",", values) : null;
            return Result;
        }

        public IList<Dictionary<string, object>> GetValues()
        {
            IList<Dictionary<string, object>> dict = new List<Dictionary<string, object>>();
            ConnectionOptions connectOptions = new ConnectionOptions();
            ManagementScope scope = new ManagementScope(@"root\cimv2");
            SelectQuery query = new SelectQuery($"SELECT {_wmiProperty} FROM {_wmiClass}");
            if (!string.IsNullOrEmpty(_wmiWhere))
                query.QueryString += $" WHERE {_wmiWhere}";
            IList<string> splitProp = null;
            if (_wmiProperty.Contains(","))
                splitProp = _wmiProperty.Split(',').Select(x => x.Replace(" ", "")).ToList();

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                using (var enumer = collection.GetEnumerator())
                {
                    while (enumer.MoveNext())
                    {
                        Dictionary<string, object> d = new Dictionary<string, object>();

                        var prop = enumer.Current.Properties;
                        foreach (var item in prop)
                        {
                            try
                            {
                                d.Add(item.Name, enumer.Current[item.Name].ToString());
                            }
                            catch
                            {
                                //
                            }
                        }
                        dict.Add(d);
                    }
                }
            }

            return dict;
        }
        public bool Equals(IDeviComponent other)
        {
            return this.EqualsObject(other,new List<string>() { nameof(this.Result) });
        }

    }
}
