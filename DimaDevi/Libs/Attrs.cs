using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
