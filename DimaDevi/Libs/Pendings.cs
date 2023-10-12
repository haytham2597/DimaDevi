namespace DimaDevi.Libs
{
    internal class Pendings
    {
        private static Pendings instance;

        private Pendings()
        {
        }

        public void Add(string BaseHardware)
        {

        }
        public Pendings GetInstance()
        {
            return instance ?? (instance = new Pendings());
        }
    }
}
