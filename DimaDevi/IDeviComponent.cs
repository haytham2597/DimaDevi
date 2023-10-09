using System;

namespace DimaDevi
{
    public interface IDeviComponent : IEquatable<IDeviComponent>
    {
        Func<string, string> Replacement { set;get; }
        /// <summary>
        /// What TYPE of hardware is. Example: "CPU", "Ram", etc.
        /// </summary>
        string BaseHardware { set;get; }
        /// <summary>
        /// Example: "Firmware Revision"
        /// </summary>
        string Name { get; }
        string GetValue();

    }
}
