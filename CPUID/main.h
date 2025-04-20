#pragma once

#ifndef MAIN_H
#define MAIN_H

#define WIN32_LEAN_AND_MEAN
#include <iostream>
#include <windows.h>


#include "cpuinfo.h"

#ifdef _USRDLL
#define CPUID_API __declspec(dllexport)
#else
#define CPUID_API __declspec(dllimport)
#endif


#ifdef _USRDLL
#ifdef __cplusplus
extern "C" {
#endif

	inline CPUID_API int* cpuid_vec(int& len)
	{
        const cpuid::cpuinfo cpuinf;
        len = 30;
        int* v = new int[len];
        v[0] = cpuinf.has_fpu();
        v[1] = cpuinf.has_mmx();
        v[2] = cpuinf.has_sse();
        v[3] = cpuinf.has_sse2();
        v[4] = cpuinf.has_sse3();
        v[5] = cpuinf.has_ssse3();
        v[6] = cpuinf.has_sse4_1();
        v[7] = cpuinf.has_sse4_2();
        v[8] = cpuinf.has_pclmulqdq();
        v[9] = cpuinf.has_avx();
        v[10] = cpuinf.has_avx2();
        v[11] = cpuinf.has_avx512_f();
        v[12] = cpuinf.has_avx512_dq();
        v[13] = cpuinf.has_avx512_ifma();
        v[14] = cpuinf.has_avx512_pf();
        v[15] = cpuinf.has_avx512_er();
        v[16] = cpuinf.has_avx512_cd();
        v[17] = cpuinf.has_avx512_bw();
        v[18] = cpuinf.has_avx512_vl();
        v[19] = cpuinf.has_avx512_vbmi();
        v[20] = cpuinf.has_avx512_vbmi2();
        v[21] = cpuinf.has_avx512_vnni();
        v[22] = cpuinf.has_avx512_bitalg();
        v[23] = cpuinf.has_avx512_vpopcntdq();
        v[24] = cpuinf.has_avx512_4vnniw();
        v[25] = cpuinf.has_avx512_4fmaps();
        v[26] = cpuinf.has_avx512_vp2intersect();
        v[27] = cpuinf.has_f16c();
        v[28] = cpuinf.has_aes();
        v[29] = cpuinf.has_neon();
        return v;
	}

#ifdef __cplusplus
}
#endif

#endif

#endif