using System;
using DimaDevi;
using DimaDevi.Formatters;

namespace DimaDeviTest.Examples
{
    public class BaseX
    {
        public BaseX()
        {
            using (DeviBuild devi = new DeviBuild())
            {
                devi.AddCPU().AddDisk().AddMotherboard();
                devi.Formatter = new BaseXForm(BaseXForm.Base.Hexadecimal);
                foreach (var value in Enum.GetValues(typeof(BaseXForm.Base)))
                {
                    devi.Formatter = new BaseXForm((BaseXForm.Base)value);
                    var content = devi.ToString();
                    var decode = devi.DecryptionDecode(content);
                    Console.WriteLine($"Base Type: {(BaseXForm.Base)value}");
                    Console.WriteLine($"Content: {content}");
                    Console.WriteLine($"Decode: {decode}");
                }
            }
        }
    }
}
