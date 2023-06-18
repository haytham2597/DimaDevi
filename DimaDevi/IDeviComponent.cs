namespace DimaDevi
{
    public interface IDeviComponent
    {
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
