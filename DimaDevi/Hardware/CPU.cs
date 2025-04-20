using System;

namespace DimaDevi.Hardware
{
    public sealed class CPUID
    {
        public CPUID(int[] vec)
        {
            var f = this.GetType().GetFields();
            /*if(vec.Length != f.Length)
                throw new Exception("Not match vec");*/
            for (int i = 0; i < f.Length; i++)
                f[i].SetValue(this, vec[i]);
        }
        //https://github.com/steinwurf/cpuid
        public int has_fpu;
        public int has_mmx;
        public int has_sse;
        public int has_sse2;
        public int has_sse3;
        public int has_ssse3;
        public int has_sse4_1;
        public int has_sse4_2;
        public int has_pclmulqdq;
        public int has_avx;
        public int has_avx2;
        public int has_avx512_f;
        public int has_avx512_dq;
        public int has_avx512_ifma;
        public int has_avx512_pf;
        public int has_avx512_er;
        public int has_avx512_cd;
        public int has_avx512_bw;
        public int has_avx512_vl;
        public int has_avx512_vbmi;
        public int has_avx512_vbmi2;
        public int has_avx512_vnni;
        public int has_avx512_bitalg;
        public int has_avx512_vpopcntdq;
        public int has_avx512_4vnniw;
        public int has_avx512_4fmaps;
        public int has_avx512_vp2intersect;
        public int has_f16c;
        public int has_aes;
        public int has_neon;
        /*
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
        public bool _3DNOW;*/
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
