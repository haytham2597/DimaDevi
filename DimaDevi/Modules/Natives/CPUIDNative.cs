using System;
using System.Runtime.InteropServices;

namespace DimaDevi.Modules.Natives
{
    public static class CPUIDNative
    {
        [DllImport("cpuid", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cpuid_vec(ref int len);

        public static int[] GetArrayInt(IntPtr ptr, int len, int offset = 0)
        {
            int[] res = new int[len];
            Marshal.Copy(ptr, res, offset, len);
            return res;
        }
        internal const uint LOAD_FLAGS_PASS_IMAGE_CHECK = 0x40000000;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int LdrLoadDllMemoryExWDelegate(
            [Out] out IntPtr BaseAddress,
            [Out] out IntPtr LdrEntry,
            [In] uint dwFlags,
            [In][MarshalAs(UnmanagedType.LPArray)] byte[] BufferAddress,
            [In] UIntPtr BufferSize,
            [In][MarshalAs(UnmanagedType.LPWStr)] string DllName,
            [In][MarshalAs(UnmanagedType.LPWStr)] string DllFullName
        );

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool LdrUnloadDllMemoryDelegate([In] IntPtr BaseAddress);
    }
}
