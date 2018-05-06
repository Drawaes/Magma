using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Magma.NetMap
{
    internal class Unix
    {
        [DllImport("libc", EntryPoint = "open")]
        public static extern int Open([MarshalAs(UnmanagedType.LPStr)] string fileName, int flags);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        public unsafe static extern int IOCtl(int descriptor, uint request, void* data);

        [Flags]
        public enum MemoryMappedProtections
        {
            PROT_NONE = 0x0,
            PROT_READ = 0x1,
            PROT_WRITE = 0x2,
            PROT_EXEC = 0x4
        }

        [Flags]
        public enum MemoryMappedFlags
        {
            MAP_SHARED = 0x01,
            MAP_PRIVATE = 0x02,
            MAP_ANONYMOUS = 0x20,
        }

        [DllImport("libc.so.6", SetLastError = true, EntryPoint = "mmap")]
        public static extern IntPtr MMap(IntPtr addr, UIntPtr length, MemoryMappedProtections prot, MemoryMappedFlags flags, int fd, UIntPtr offset);
    }
}
