using System;

namespace DimaDevi.Libs
{
    public class Attrs
    {
        public class MethodNameAttribute : Attribute
        {
            public string MethodName;
            public MethodNameAttribute(string method_name)
            {
                this.MethodName = method_name;
            }
        }
    }
}
