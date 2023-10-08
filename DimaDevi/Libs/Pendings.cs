using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
