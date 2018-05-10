using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Magma.NetMap.Interop
{
    public class NetMapInterop : IDisposable
    {
        const ushort NETMAP_RING_MASK = 0x0fff;	/* the ring number */
        
        const ushort NETMAP_NO_TX_POLL = 0x1000;	/* no automatic txsync on poll */
        const ushort NETMAP_DO_RX_POLL = 0x8000;
        
        const ushort NR_REG_MASK = 0xf; /* to extract NR_REG_* mode from nr_flags */

        
        
        
               

        //public static unsafe nm_desc nm_open(string ifname, nmreq req, ulong flags, void* arg)
        //{
        //    nr_mode nr_reg;

        //    var ptrToDescription = Marshal.AllocHGlobal(Unsafe.SizeOf<nm_desc>());

        //    var d = Unsafe.AsRef<nm_desc>(ptrToDescription.ToPointer());
        //    d.self = ptrToDescription;

        //    var fd = Unix.Open("/dev/netmap", Unix.OpenFlags.O_RDWR);
        //    if (fd < 0) throw new InvalidOperationException("Need to handle properly (release memory etc)");

        //    d.req.nr_flags = (uint)nr_mode.NR_REG_ALL_NIC;
        //    d.req.nr_arg1 = 0;
        //    var textbytes = Encoding.ASCII.GetBytes(ifname + "\0");
        //    fixed (void* txtPtr = textbytes)
        //    {
        //        Buffer.MemoryCopy(txtPtr, d.req.nr_name, textbytes.Length, textbytes.Length);
        //    }

        //    d.req.nr_version = NETMAP_API;
        //    d.req.nr_ringid &= NETMAP_RING_MASK;

        //    /* add the *XPOLL flags */
        //    d.req.nr_ringid |= (ushort)(flags & (NETMAP_NO_TX_POLL | NETMAP_DO_RX_POLL));
        //    Console.WriteLine($"Ring id was {d.req.nr_ringid}");
        //    d.req.nr_flags = 0x8001;
        //    d.req.nr_cmd = 0;

        //    if (Unix.IOCtl(d.fd, NIOCREGIF, &d.req) != 0)
        //    {
        //        Console.WriteLine($"Error with the IO CTL error code was {Marshal.GetLastWin32Error()}");
        //        throw new InvalidOperationException("Some failure to get the port, need better error handling");
        //    }

        //    nr_reg = (nr_mode)(d.req.nr_flags & NR_REG_MASK);

        //    if (nr_reg == nr_mode.NR_REG_SW)
        //    { /* host stack */
        //        d.first_tx_ring = d.last_tx_ring = d.req.nr_tx_rings;
        //        d.first_rx_ring = d.last_rx_ring = d.req.nr_rx_rings;
        //    }
        //    else if (nr_reg == nr_mode.NR_REG_ALL_NIC)
        //    { /* only nic */
        //        d.first_tx_ring = 0;
        //        d.first_rx_ring = 0;
        //        d.last_tx_ring = (ushort)(d.req.nr_tx_rings - 1);
        //        d.last_rx_ring = (ushort)(d.req.nr_rx_rings - 1);
        //    }
        //    else if (nr_reg == nr_mode.NR_REG_NIC_SW)
        //    {
        //        d.first_tx_ring = 0;
        //        d.first_rx_ring = 0;
        //        d.last_tx_ring = d.req.nr_tx_rings;
        //        d.last_rx_ring = d.req.nr_rx_rings;
        //    }
        //    else if (nr_reg == nr_mode.NR_REG_ONE_NIC)
        //    {
        //        /* XXX check validity */
        //        d.first_tx_ring = d.last_tx_ring =
        //        d.first_rx_ring = d.last_rx_ring = (ushort)(d.req.nr_ringid & NETMAP_RING_MASK);
        //    }
        //    else
        //    { /* pipes */
        //        d.first_tx_ring = d.last_tx_ring = 0;
        //        d.first_rx_ring = d.last_rx_ring = 0;
        //    }

        //    d.cur_tx_ring = d.first_tx_ring;
        //    d.cur_rx_ring = d.first_rx_ring;
        //    return d;
        //}

        public void Dispose() => throw new NotImplementedException();
    }
}