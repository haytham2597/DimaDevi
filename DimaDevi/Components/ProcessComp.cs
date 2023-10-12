using DimaDevi.Libs.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimaDevi.Libs;
using static DimaDevi.Libs.Enumerations;

namespace DimaDevi.Components
{
    internal class ProcessComp : IDeviComponent
    {
        private string Result = string.Empty;
        public Func<string, string> Replacement { get; set; }
        public string BaseHardware { get; set; }
        public string Name { get; }
        public ProcessComp(Process proc)
        {
            this.Name = proc.MainWindowTitle;
            if (Environment.Is64BitProcess)
            {
                var m = proc.MainModule;
                if (m == null)
                    return;
                Result = $"{m.FileName}:{m.FileVersionInfo.FileVersion}:{m.ModuleName}:{m.ModuleMemorySize}";
            }
            else
            {
                var wmproc = new WMIComp("Proc", "Win32_Process", "Caption, Description, PageFaults", "*", $"Handle={proc.Id}");
                Result = wmproc.GetValue();
            }
            //Result = $"{m.FileName}:{m.FileVersionInfo.FileVersion}:{m.ModuleName}:{m.ModuleMemorySize}";
        }

        public ProcessComp(Process proc, Enumerations.ProcessInfo procInfo)
        {
            var enumType = procInfo.GetType();
            var gca = enumType.GetCustomAttributes(typeof(Attrs.WMINameAttribute), true);
            var flags = procInfo.GetFlags();
            string wmProp = string.Empty;
            using (var enumer = flags.GetEnumerator())
            {
                while (enumer.MoveNext())
                {
                    if (Convert.ToInt32(enumer.Current) == -1) //is ALL
                        continue;
                    if (enumer.Current == null)
                        continue;
                    var memberInfos = enumType.GetMember(enumer.Current.ToString());
                    var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                    var valueAttributes = enumValueMemberInfo?.GetCustomAttributes(typeof(Attrs.WMINameAttribute), false);
                    wmProp += valueAttributes?.Length == 0 ? enumer.Current.ToString() : (valueAttributes?[0] as Attrs.WMINameAttribute)?.Name;
                    wmProp += ", ";
                    //devi.AddComponents(new WMIComp(enumer.Current.ToString(), (gca[0] as Attrs.WMINameAttribute)?.Name, valueAttributes?.Length == 0 ? enumer.Current.ToString() : (valueAttributes?[0] as Attrs.WMINameAttribute)?.Name) { BaseHardware = enumType.Name });
                }
            }
            wmProp = wmProp.TrimEnd(',', ' ');
            var wmProc = new WMIComp("Proccess", "Win32_Process", wmProp, "*", $"Handle={proc.Id}");
            Result = wmProc.GetValue();
        }
        public string GetValue()
        {
            return Replacement != null ? Replacement(Result) : Result;
        }
        public bool Equals(IDeviComponent other)
        {
            return this.EqualsObject(other, new List<string>(){nameof(this.Result)});
        }
    }
}
