namespace DimaDevi
{
    public interface IDeviComponent
    {
        /// <summary>
        /// What type of hardware is. Example: "CPU", "Ram", etc.
        /// </summary>
        string BaseHardware { set;get; }
        string Name { get; }
        string GetValue();

    }
}
