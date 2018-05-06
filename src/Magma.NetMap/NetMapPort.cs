using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Magma.NetMap.Interop;

namespace Magma.NetMap
{
    public class NetMapPort
    {
        private NetMapRingPair[] _rings;
        private string _interfaceName;
        private nmreq _request;
        private int _fileDescriptor;
        private IntPtr _mappedRegion;
        private netmap_if _netmapInterface;

        const ushort NETMAP_API = 12;
        const uint NIOCREGIF = 0xC03C6992;

        public NetMapPort(string interfaceName) => _interfaceName = interfaceName;

        private IntPtr NetMapInterfaceAddress => IntPtr.Add(_mappedRegion, (int)_request.nr_offset);

        public unsafe void Open()
        {
            var request = new nmreq
            {
                nr_cmd = 0,
                nr_flags = 0x8003,
                nr_ringid = 0,
                nr_version = NETMAP_API,
            };
            var textbytes = Encoding.ASCII.GetBytes(_interfaceName + "\0");
            fixed (void* txtPtr = textbytes)
            {
                Buffer.MemoryCopy(txtPtr, request.nr_name, textbytes.Length, textbytes.Length);
            }
            _fileDescriptor = Unix.Open("/dev/netmap", Unix.OpenFlags.O_RDWR);
            if (_fileDescriptor < 0) throw new InvalidOperationException("Need to handle properly (release memory etc)");
            if (Unix.IOCtl(_fileDescriptor, NIOCREGIF, &request) != 0)
            {
                throw new InvalidOperationException("Some failure to get the port, need better error handling");
            }
            _request = request;
            MapMemory();
            SetupRings();
        }

        private unsafe void SetupRings()
        {
            var txOffsets = new ulong[_netmapInterface.ni_tx_rings];
            var rxOffsets = new ulong[_netmapInterface.ni_rx_rings];
            var span = new Span<byte>(IntPtr.Add(NetMapInterfaceAddress, Unsafe.SizeOf<netmap_if>()).ToPointer(), (int)((_netmapInterface.ni_rx_rings + _netmapInterface.ni_tx_rings + 2) * sizeof(IntPtr)));
            for (var i = 0; i < txOffsets.Length; i++)
            {
                txOffsets[i] = System.Buffers.Binary.BinaryPrimitives.ReadUInt64LittleEndian(span);
                span = span.Slice(sizeof(ulong));
            }
            var txHost = System.Buffers.Binary.BinaryPrimitives.ReadUInt64LittleEndian(span);
            span = span.Slice(sizeof(ulong));
            
            for (var i = 0; i < rxOffsets.Length; i++)
            {
                rxOffsets[i] = System.Buffers.Binary.BinaryPrimitives.ReadUInt64LittleEndian(span);
                span = span.Slice(sizeof(ulong));
            }
            var rxHost = System.Buffers.Binary.BinaryPrimitives.ReadUInt64LittleEndian(span);

            _rings = new NetMapRingPair[rxOffsets.Length];
            for(var i = 0; i < rxOffsets.Length;i++)
            {
                _rings[i] = new NetMapRingPair((byte*)_mappedRegion.ToPointer(), rxOffsets[i], txOffsets[i]);
            }
        }

        private unsafe void MapMemory()
        {
            var mapResult = Unix.MMap(IntPtr.Zero, _request.nr_memsize, Unix.MemoryMappedProtections.PROT_READ | Unix.MemoryMappedProtections.PROT_WRITE, Unix.MemoryMappedFlags.MAP_SHARED, _fileDescriptor, offset: 0);

            if ((long)mapResult < 0) throw new InvalidOperationException("Failed to map the memory");

            Console.WriteLine("Mapped the memory region correctly");
            _mappedRegion = mapResult;
            _netmapInterface = Unsafe.Read<netmap_if>(NetMapInterfaceAddress.ToPointer());
        }
        
        public void PrintPortInfo()
        {
            Console.WriteLine($"memsize = {_request.nr_memsize}");
            Console.WriteLine($"tx rings = {_request.nr_tx_rings}");
            Console.WriteLine($"rx rings = {_request.nr_rx_rings}");
            Console.WriteLine($"tx slots = {_request.nr_tx_slots}");
            Console.WriteLine($"rx slots = {_request.nr_rx_slots}");
            Console.WriteLine($"Offset to IF Header {_request.nr_offset}");
            Console.WriteLine($"Interface Nic RX Queues {_netmapInterface.ni_rx_rings}");
            Console.WriteLine($"Interface Nic TX Queues {_netmapInterface.ni_tx_rings}");
            Console.WriteLine($"Interface Start of extra buffers {_netmapInterface.ni_bufs_head}");
        }
    }
}