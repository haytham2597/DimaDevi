using System;
using System.Reflection;

namespace DimaDevi.Libs
{
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    internal class Attrs
    {
        public class MethodNameAttribute : Attribute
        {
            public string MethodName;
            public MethodNameAttribute(string method_name)
            {
                MethodName = method_name;
            }
        }

        public class WMINameAttribute : Attribute
        {
            public string Name;
            public WMINameAttribute(string name)
            {
                Name = name;
            }
        }
    }
}
