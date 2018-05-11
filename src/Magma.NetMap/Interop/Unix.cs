using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Magma.NetMap
{
    internal class Unix
    {
        [DllImport("libc", EntryPoint = "open")]
        public static extern int Open([MarshalAs(UnmanagedType.LPStr)] string fileName, OpenFlags flags);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        public unsafe static extern int IOCtl(int descriptor, uint request, void* data);

        [DllImport("libc", EntryPoint = "close")]
        public unsafe static extern int Close(int fd);

        public enum EPOLL_EVENTS : uint
        {
            EPOLLIN = 0x001,
            EPOLLPRI = 0x002,
            EPOLLOUT = 0x004,
            EPOLLRDNORM = 0x040,
            EPOLLRDBAND = 0x080,
            EPOLLWRNORM = 0x100,
            EPOLLWRBAND = 0x200,
            EPOLLMSG = 0x400,
            EPOLLERR = 0x008,
            EPOLLHUP = 0x010,
            EPOLLRDHUP = 0x2000,
            EPOLLEXCLUSIVE = 1u << 28,
            EPOLLWAKEUP = 1u << 29,
            EPOLLONESHOT = 1u << 30,
            EPOLLET = 1u << 31
        };

        [DllImport("libc", EntryPoint = "epoll_ctl")]
        public unsafe static extern int epoll_ctl(int epfd, int op, int fd, void* epEvent);

        [Flags]
        public enum OpenFlags
        {
            O_RDONLY = 0x0000,      /* open for reading only */
            O_WRONLY = 0x0001,      /* open for writing only */
            O_RDWR = 0x0002,        /* open for reading and writing */
            O_ACCMODE = 0x0003,     /* mask for above modes */
        }

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

        [DllImport("libc", SetLastError = true, EntryPoint = "mmap")]
        public static extern IntPtr MMap(IntPtr addr, ulong length, MemoryMappedProtections prot, MemoryMappedFlags flags, int fd, ulong offset);

        [DllImport("libc", EntryPoint = "munmap")]
        public static extern int MUnmap(IntPtr addr, ulong length);

        [DllImport("libc", EntryPoint = "poll")]
        public static extern int poll(ref pollFd pollfd, int numberOfFileDescriptors, int timeout);

        [StructLayout(LayoutKind.Sequential)]
        public struct pollFd
        {
            public int Fd;
            public PollEvents Events;
            public PollEvents Revents;
        }

        [Flags]
        public enum PollEvents : short
        {
            POLLIN = 0x0001, // There is data to read
            POLLPRI = 0x0002, // There is urgent data to read
            POLLOUT = 0x0004, // Writing now will not block
            POLLERR = 0x0008, // Error condition
            POLLHUP = 0x0010, // Hung up
            POLLNVAL = 0x0020, // Invalid request; fd not open
                               // XPG4.2 definitions (via _XOPEN_SOURCE)
            POLLRDNORM = 0x0040, // Normal data may be read
            POLLRDBAND = 0x0080, // Priority data may be read
            POLLWRNORM = 0x0100, // Writing now will not block
            POLLWRBAND = 0x0200, // Priority data may be written
        }
    }
}
