using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using DimaDevi.Libs;
using DimaDevi.Libs.Extensions;

namespace DimaDevi.Components
{
    public class AssemblyComp : IDeviComponent
    {
        public Func<string, string> Replacement { get; set; }
        public string BaseHardware { get; set; } = nameof(AssemblyComp);
        public string Name { get; } = "Assembly";
        private Assembly _Assembly { set; get; }
        private Enumerations.AssemblyEn AssemblyEn { set; get; }

        public AssemblyComp(Assembly assembly)
        {
            _Assembly = assembly;
        }

        public AssemblyComp(Assembly assembly, Enumerations.AssemblyEn assemblyEnum) : this(assembly)
        {
            AssemblyEn = assemblyEnum;
        }
        
        public string GetValue()
        {
            var entry = Assembly.GetEntryAssembly();
            if (_Assembly != null)
                entry = _Assembly;

            List<string> allInfoResultAssembly = new List<string>();
            if (AssemblyEn.HasFlag(Enumerations.AssemblyEn.PublicKeyToken))
                allInfoResultAssembly.Add(entry.GetPublicKeyTokenFromAssembly());
            var fullname = entry.GetFullNameDictFromAssembly();
            var flags = AssemblyEn.GetFlags();
            foreach (var d in fullname)
            {
                if (!Enum.IsDefined((typeof(Enumerations.AssemblyEn)), d.Key))
                    continue;
                if (flags.Contains((Enumerations.AssemblyEn)Enum.Parse(typeof(Enumerations.AssemblyEn), d.Key)))
                    allInfoResultAssembly.Add(d.Value);
            }
            if (AssemblyEn.HasFlag(Enumerations.AssemblyEn.CreationDate) && !string.IsNullOrEmpty(entry?.Location) && File.Exists(entry.Location))
                allInfoResultAssembly.Add(File.GetCreationTime(entry.Location).ToString(CultureInfo.InvariantCulture));
            if (AssemblyEn.HasFlag(Enumerations.AssemblyEn.Hash) && !string.IsNullOrEmpty(entry?.Location) && File.Exists(entry.Location))
                allInfoResultAssembly.Add(new FileComp(entry.Location).GetValue());
            string result = string.Join(",", allInfoResultAssembly);
            if (Replacement != null)
                result = Replacement(result);
            return (allInfoResultAssembly.Count > 0) ? result : null;
        }

        public bool Equals(IDeviComponent other)
        {

            return this.EqualsObject(other);
        }
    }
}
