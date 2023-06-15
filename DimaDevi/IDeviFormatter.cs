using System;
using System.Collections.Generic;

namespace DimaDevi
{
    public interface IDeviFormatter : IDisposable
    {
        bool PreventComponentDuplication { set; get; }
        string GetDevi(IEnumerable<IDeviComponent> components);
        string GetDevi(string componentsResult, string separator);
    }
}
