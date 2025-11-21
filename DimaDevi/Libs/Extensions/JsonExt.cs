using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DimaDevi.Libs.Extensions
{
    public static class JsonExt
    {
        /// <summary>
        /// Supporting for old version Json
        /// </summary>
        /// <param name="jo"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool JObjectContains(this JObject jo, string name)
        {
#if (DEBUGJS45 || RELEASEJS45)
            return jo.Property(name) != null || jo[name] != null;
#else
            return jo.ContainsKey(name);
#endif
        }
    }
}
