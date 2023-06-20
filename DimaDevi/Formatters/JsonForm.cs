using System.Linq;
using System.Collections.Generic;
using DimaDevi.Libs;
using Newtonsoft.Json.Linq;

namespace DimaDevi.Formatters
{
    public sealed class JsonForm : IDeviFormatter
    {
        private Newtonsoft.Json.Formatting Formatting { set; get; }
        public JsonForm(Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.None)
        {
            Formatting = formatting;
        }

        public string GetDevi(IEnumerable<IDeviComponent> components)
        {
            JObject o = new JObject();
            JObject jo = new JObject();
            
            if (GeneralConfigs.GetInstance().PreventDuplicationComponents)
                components= components.DistinctBy(x => x.BaseHardware).Where(x => !string.IsNullOrEmpty(x.Name));
            using (var enumer = components.GetEnumerator())
                while(enumer.MoveNext())
                    jo.Add(enumer.Current?.Name, enumer.Current?.GetValue());

            o["Components"] = jo;
            return o.ToString(Formatting);
        }

        public string GetDevi(string componentsResult, string separator)
        {
            var spl = componentsResult.Split(separator.ToCharArray());

            JObject o = new JObject();
            JObject jo = new JObject();
            for (int i = 0; i < spl.Length; i++)
            {
                var splElem = spl[i].Split('=');
                if (splElem.Length != 2)
                    continue;
                if (jo.ContainsKey(splElem[0]))
                {
                    int n = 0;
                    do
                    {
                        n++;
                        splElem[0] += n.ToString();
                    } while (jo.ContainsKey(splElem[0]));
                }

                jo.Add(splElem[0], splElem[1]); //Cause exception if the name is same
                
            }
            o["Components"] = jo;
            return o.ToString(Formatting);
        }
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
