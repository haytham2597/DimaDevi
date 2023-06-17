using System;
using System.Collections.Generic;
using System.Management;
using DimaDevi.Libs;

namespace DimaDevi.Components
{
    /// <summary>
    /// An implementation of <see cref="IDeviComponent"/> that retrieves data from a WMI class.
    /// </summary>
    public sealed class WMIComp : IDeviComponent
    {
        public string BaseHardware { set; get; } = null;
        public string Name { get; }
        /// <summary>
        /// The WMI class name.
        /// </summary>
        private readonly string _wmiClass;

        /// <summary>
        /// The WMI property name.
        /// </summary>
        private readonly string _wmiProperty;
        private readonly string _wmiName;
        private readonly string _wmiWhere;
        //public Property.RemoteWMICredential WmiCredential;

        /// <summary>
        /// Initializes a new instance of the <see cref="WMIComp"/> class.
        /// SELECT wmiProperty FROM wmiClass
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <param name="wmiClass">The WMI class name.</param>
        /// <param name="wmiProperty">The WMI property name.</param>
        /// <param name="wmiCredential"></param>
        public WMIComp(string name, string wmiClass, string wmiProperty)
        {
            Name = name;
            _wmiClass = wmiClass;
            _wmiProperty = wmiProperty;
            /*if (wmiCredential != null)
                WmiCredential = wmiCredential;*/
        }
        public WMIComp(string name, string wmiClass, string wmiProperty, string wmiName) : this(name, wmiClass, wmiProperty)
        {
            _wmiName = wmiName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="wmiClass"></param>
        /// <param name="wmiProperty"></param>
        /// <param name="wmiName"></param>
        /// <param name="wmiWhere">Example: DeviceID=value, will find in DeviceID that contains this vlaue</param>
        /// <param name="wmiCredential"></param>
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
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(_wmiClass) || string.IsNullOrEmpty(_wmiProperty))
                return null;

            var values = new List<string>();
            try
            {
                ConnectionOptions connectOptions = new ConnectionOptions();
                ManagementScope scope = new ManagementScope(@"root\cimv2");
                
                if (GeneralConfigs.GetInstance().RemoteWmi != null && !GeneralConfigs.GetInstance().RemoteWmi.IsEmpty())
                {
                    connectOptions.Username = GeneralConfigs.GetInstance().RemoteWmi.Username;
                    connectOptions.Password = GeneralConfigs.GetInstance().RemoteWmi.Password;
                    scope.Path = new ManagementPath(GeneralConfigs.GetInstance().RemoteWmi.Domain.AddTwoBackSlashIfIsPossible() + @"\root\cimv2");
                    scope.Options = connectOptions;
                }
                SelectQuery query = new SelectQuery($"SELECT {_wmiProperty} FROM {_wmiClass}");
                using (ManagementObjectSearcher searcher =new ManagementObjectSearcher(scope, query))
                using (ManagementObjectCollection collection = searcher.Get())
                {
                    foreach (var mo in collection)
                    {
                        if (!string.IsNullOrEmpty(_wmiWhere))
                        {
                            var spl = _wmiWhere.Split('=');
                            if (mo[spl[0]].ToString().Contains(spl[1]))
                            {
                                values.Add(mo[_wmiProperty == "*" ? _wmiName : _wmiProperty].ToString());
                                break;
                            }
                            continue;
                        }
                        values.Add(mo[_wmiProperty == "*" ? _wmiName : _wmiProperty].ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            values.Sort();
            return (values.Count > 0) ? string.Join(",", values) : null;
        }
    }
}
