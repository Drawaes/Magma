using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Magma.Internet.Icmp;

namespace Magma.Network.Header
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IcmpV4
    {
        private short _typeAndCode;

        public ControlMessage Type
        {
            get => (ControlMessage)(_typeAndCode & 0xFF);
            set => _typeAndCode = (short)((byte)value | (_typeAndCode & 0xFF00));
        }
        public Code Code
        {
            get => (Code)_typeAndCode;
            set => _typeAndCode = (short)value;
        }

        public ushort HeaderChecksum;
        public short Identifier;
        public short SequenceNumber;

        public static bool TryConsume(ReadOnlySpan<byte> input, out IcmpV4 icmp, out ReadOnlySpan<byte> data)
        {
            if (input.Length >= Unsafe.SizeOf<IcmpV4>())
            {
                icmp = Unsafe.As<byte, IcmpV4>(ref MemoryMarshal.GetReference(input));
                // CRC check
                data = input.Slice(Unsafe.SizeOf<IcmpV4>());
                return true;
            }

            data = default;
            icmp = default;
            return false;
        }

        public unsafe override string ToString()
        {
            return "+- Icmp Datagram ----------------------------------------------------------------------+" + Environment.NewLine +
                  $"| {Type.ToString()} - {Code} | Id: {System.Net.IPAddress.NetworkToHostOrder(Identifier)} | Seq: {System.Net.IPAddress.NetworkToHostOrder(SequenceNumber)} ".PadRight(87) 
                  + "|";
        }

        public static bool IsChecksumValid(ref byte IcmpStart, int length) => Checksum.IsValid(ref IcmpStart, length);
    }
}
