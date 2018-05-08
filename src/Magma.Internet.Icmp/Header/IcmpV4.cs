using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Magma.Internet.Icmp;

namespace Magma.Network.Header
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IcmpV4
    {
        private ushort _typeAndCode;

        public ControlMessage Type
        {
            get => (ControlMessage)(_typeAndCode & 0xF);
        }
        public Code Code
        {
            get => (Code)_typeAndCode;
        }

        public short HeaderChecksum;
        public short Identifier;
        public short SequenceNumber;
        public int Payload;

        public static bool TryConsume(ref Span<byte> span, out IcmpV4 icmp)
        {
            if (span.Length >= Unsafe.SizeOf<IcmpV4>())
            {
                icmp = Unsafe.As<byte, IcmpV4>(ref MemoryMarshal.GetReference(span));
                // CRC check
                return true;
            }

            icmp = default;
            return false;
        }

        public override string ToString()
        {
            return "+- Icmp Datagram ----------------------------------------------------------------------+" + Environment.NewLine +
                  $"| {Type.ToString()} - {Code} ".PadRight(87) + "|";
        }
    }
}
