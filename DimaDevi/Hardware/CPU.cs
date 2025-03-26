namespace DimaDevi.Hardware
{
    public sealed class CPUID
    {
        //https://github.com/steinwurf/cpuid
        public bool SSE3;
        public bool PCLMULQDQ;
        public bool MONITOR;
        public bool SSSE3;
        public bool FMA;
        public bool CMPXCHG16B;
        public bool SSE41;
        public bool SSE42;
        public bool MOVBE;
        public bool POPCNT;
        public bool AES;
        public bool XSAVE;
        public bool OSXSAVE;
        public bool AVX;
        public bool F16C;
        public bool RDRAND;

        public bool MSR;
        public bool CX8;
        public bool SEP;
        public bool CMOV;
        public bool CLFSH;
        public bool MMX;
        public bool FXSR;
        public bool SSE;
        public bool SSE2;

        public bool FSGSBASE;
        public bool BMI1;
        public bool HLE;
        public bool AVX2;
        public bool BMI2;
        public bool ERMS;
        public bool INVPCID;
        public bool RTM;
        public bool AVX512F;
        public bool RDSEED;
        public bool ADX;
        public bool AVX512PF;
        public bool AVX512ER;
        public bool AVX512CD;
        public bool SHA;

        public bool PREFETCHWT1;

        public bool LAHF;
        public bool LZCNT;
        public bool ABM;
        public bool SSE4a;
        public bool XOP;
        public bool TBM;

        public bool SYSCALL;
        public bool MMXEXT;
        public bool RDTSCP;
        public bool _3DNOWEXT;
        public bool _3DNOW;
    }
    public sealed class CPU
    {
        public string ProcessorId { set; get; }
        public string Name { set; get; }
        public int NumberOfCores { set; get; }
        public string Description { set; get; }
        public string PartNumber { set; get; }
        public int ThreadCount { set; get; }

    }
}
