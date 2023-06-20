using System;

namespace DimaDevi.Libs
{
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
    }
}
