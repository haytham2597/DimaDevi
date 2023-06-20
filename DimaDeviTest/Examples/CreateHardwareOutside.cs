using DimaDevi;
using DimaDevi.Hardware;
using DimaDevi.Libs;

namespace DimaDeviTest.Examples
{
    public class CreateHardwareOutside
    {
        public CreateHardwareOutside()
        {
            HardwareComponents.GetInstance()
                .AddComponent(typeof(Enumerations.CPU), "L3CacheSize")
                .AddComponent(typeof(Enumerations.CPU), "L2CacheSize"); 
            DeviBuild devi = new DeviBuild()
                .AddCPU(Enumerations.CPU.Description | Enumerations.CPU.Name)
                .AddMotherboard(Enumerations.Motherboard.Name | Enumerations.Motherboard.Manufacturer)
                .AddMachineName()
                .AddMacAddress()
                .AddRam(Enumerations.RAM.All)
                .AddRegistry(@"SOFTWARE\\DefaultUserEnvironment", "Path");
            string content = devi.ToString("<separ>"); //Print Description, Name CPU and L3CacheSize, L2CacheSize of CPU and other components
        }
    }
}
