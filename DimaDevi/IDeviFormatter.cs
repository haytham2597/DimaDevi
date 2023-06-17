using System;
using System.Collections.Generic;

namespace DimaDevi
{
    public interface IDeviFormatter : IDisposable
    {
        string GetDevi(IEnumerable<IDeviComponent> components);
        string GetDevi(string componentsResult, string separator);
    }
}
