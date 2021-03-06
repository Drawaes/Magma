
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Magma.Transport.Tcp.Header;

namespace Magma.Network.Header
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Tcp
    {
        private ushort _sourcePort;
        private ushort _destinationPort;
        private uint _sequenceNumber;
        private uint _acknowledgmentNumber;
        private byte _dataOffsetAndReservedAndFlags0;
        private byte _flags1;
        private ushort _windowSize;
        private ushort _checksum;
        private ushort _urgentPointer;

        /// <summary>
        /// Identifies the sending port.
        /// </summary>
        public ushort SourcePort { get => (ushort)System.Net.IPAddress.NetworkToHostOrder((short)_sourcePort); set => _sourcePort = (ushort)System.Net.IPAddress.HostToNetworkOrder((short)value); }
        /// <summary>
        /// Identifies the receiving port.
        /// </summary>
        public ushort DestinationPort { get => (ushort)System.Net.IPAddress.NetworkToHostOrder((short)_destinationPort); set => _destinationPort = (ushort)System.Net.IPAddress.HostToNetworkOrder((short)value); }

        /// <summary>
        /// Has a dual role: If the SYN flag is set(1), then this is the initial sequence number. 
        /// The sequence number of the actual first data byte and the acknowledged number in the corresponding ACK are then this sequence number plus 1.
        /// If the SYN flag is clear (0), then this is the accumulated sequence number of the first data byte of this segment for the current session.
        /// </summary>
        public uint SequenceNumber { get => (uint)System.Net.IPAddress.NetworkToHostOrder((int)_sequenceNumber); set => _sequenceNumber = (uint)System.Net.IPAddress.HostToNetworkOrder((int)value); }

        /// <summary>
        /// If the ACK flag is set then the value of this field is the next sequence number that the sender of the ACK is expecting.
        /// This acknowledges receipt of all prior bytes (if any). The first ACK sent by each end acknowledges the other end's initial sequence number itself, but no data.
        /// </summary>
        public uint AcknowledgmentNumber { get => (uint)System.Net.IPAddress.NetworkToHostOrder((int)_acknowledgmentNumber); set => _acknowledgmentNumber = (uint)System.Net.IPAddress.HostToNetworkOrder((int)value); }

        /// <summary>
        /// Specifies the size of the TCP header in 32-bit words.
        /// The minimum size header is 5 words and the maximum is 15 words thus giving the minimum size of 20 bytes and maximum of 60 bytes, 
        /// allowing for up to 40 bytes of options in the header. 
        /// This field gets its name from the fact that it is also the offset from the start of the TCP segment to the actual data.
        /// </summary>
        public byte DataOffset
        {
            get => (byte)(_dataOffsetAndReservedAndFlags0 >> 4);
            set
            {
                value = (byte)(value << 4);
                _dataOffsetAndReservedAndFlags0 = (byte)(value | (_dataOffsetAndReservedAndFlags0 & 0b0000_0001));
            }
        }
        /// <summary>
        /// For future use and should be set to zero. (3 bits)
        /// </summary>
        public byte Reserved => (byte)((_dataOffsetAndReservedAndFlags0 & 0xf) >> 1);

        public TcpFlags Flags
        {
            get => (TcpFlags)(((_dataOffsetAndReservedAndFlags0 & 0b1) << 8) | _flags1);
            set
            {
                _flags1 = (byte)value;
                if (((int)value >> 8) > 0)
                {
                    _dataOffsetAndReservedAndFlags0 |= 0b0000_0001;
                }
                else
                {
                    _dataOffsetAndReservedAndFlags0 &= 0b1111_1110;
                }
            }
        }

        /// <summary>
        /// ECN-nonce - concealment protection(experimental: see RFC 3540)
        /// </summary>
        public bool NS
        {
            get => (_dataOffsetAndReservedAndFlags0 & 0b1) == 0 ? false : true;
            set
            {
                if (value)
                {
                    _dataOffsetAndReservedAndFlags0 |= 0b0000_0001;
                }
                else
                {
                    _dataOffsetAndReservedAndFlags0 &= 0b1111_1110;
                }
            }
        }

        /// <summary>
        /// Congestion Window Reduced flag is set by the sending host to indicate that it received a TCP segment 
        /// with the ECE flag set and had responded in congestion control mechanism (added to header by RFC 3168).
        /// </summary>
        public bool CWR
        {
            get => (_flags1 & 0b_1000_0000) == 0 ? false : true;
            set
            {
                if (value)
                {
                    _flags1 |= 0b_1000_0000;
                }
                else
                {
                    _flags1 &= 0b_0111_1111;
                }
            }
        }

        /// <summary>
        /// ECN-Echo has a dual role, depending on the value of the SYN flag.
        /// It indicates: If the SYN flag is set (1), that the TCP peer is ECN capable.
        /// If the SYN flag is clear (0), that a packet with Congestion Experienced flag set(ECN= 11) in the IP header was received 
        /// during normal transmission(added to header by RFC 3168). 
        /// This serves as an indication of network congestion (or impending congestion) to the TCP sender.
        /// </summary>
        public bool ECE
        {
            get => (_flags1 & 0b_0100_0000) == 0 ? false : true;
            set
            {
                if (value)
                {
                    _flags1 |= 0b_0100_0000;
                }
                else
                {
                    _flags1 &= 0b_1011_1111;
                }
            }
        }

        /// <summary>
        /// Indicates that the Urgent pointer field is significant
        /// </summary>
        public bool URG
        {
            get => (_flags1 & 0b_0010_0000) == 0 ? false : true;
            set
            {
                if (value)
                {
                    _flags1 |= 0b_0010_0000;
                }
                else
                {
                    _flags1 &= 0b_1101_1111;
                }
            }
        }
        /// <summary>
        /// Indicates that the Acknowledgment field is significant.
        /// All packets after the initial SYN packet sent by the client should have this flag set.
        /// </summary>
        public bool ACK
        {
            get => (_flags1 & 0b_0001_0000) == 0 ? false : true;
            set
            {
                if (value)
                {
                    _flags1 |= 0b_0001_0000;
                }
                else
                {
                    _flags1 &= 0b1110_1111;
                }
            }
        }

        /// <summary>
        /// Push function. Asks to push the buffered data to the receiving application.
        /// </summary>
        public bool PSH
        {
            get => (_flags1 & 0b_0000_1000) == 0 ? false : true;
            set
            {
                if (value)
                {
                    _flags1 |= 0b_0000_1000;
                }
                else
                {
                    _flags1 &= 0b_1111_0111;
                }
            }
        }

        /// <summary>
        /// Reset the connection
        /// </summary>
        public bool RST
        {
            get => (_flags1 & 0b_0000_0100) == 0 ? false : true;
            set
            {
                if (value)
                {
                    _flags1 |= 0b_0000_0100;
                }
                else
                {
                    _flags1 &= 0b_1111_1011;
                }
            }
        }
        /// <summary>
        /// Synchronize sequence numbers.
        /// Only the first packet sent from each end should have this flag set.
        /// Some other flags and fields change meaning based on this flag, and some are only valid when it is set, and others when it is clear.
        /// </summary>
        public bool SYN
        {
            get => (_flags1 & 0b_0000_0010) == 0 ? false : true;
            set
            {
                if (value)
                {
                    _flags1 |= 0b_0000_0010;
                }
                else
                {
                    _flags1 &= 0b_1111_1101;
                }
            }
        }

        /// <summary>
        /// Last packet from sender.
        /// </summary>
        public bool FIN
        {
            get => (_flags1 & 0b_0000_0001) == 0 ? false : true;
            set
            {
                if (value)
                {
                    _flags1 |= 0b_0000_0001;
                }
                else
                {
                    _flags1 &= 0b_1111_1110;
                }
            }
        }

        /// <summary>
        /// The size of the receive window, which specifies the number of bytes that 
        /// the sender of this segment is currently willing to receive.
        /// </summary>
        public ushort WindowSize { get => (ushort)System.Net.IPAddress.NetworkToHostOrder((short)_windowSize); set => _windowSize = (ushort)System.Net.IPAddress.HostToNetworkOrder((short)value); }

        /// <summary>
        /// The 16-bit checksum field is used for error-checking of the header, the Payload and a Pseudo-Header.
        /// The Pseudo-Header consists of the Source IP Address, the Destination IP Address, 
        /// the protocol number for the TCP-Protocol (0x0006) and the length of the TCP-Headers including Payload (in Bytes).
        /// </summary>
        public ushort Checksum { get => _checksum; set => _checksum = value; }

        /// <summary>
        /// If the URG flag is set, then this 16-bit field is an offset from the sequence number indicating the last urgent data byte.
        /// </summary>
        /// <returns></returns>
        public ushort UrgentPointer { get => (ushort)System.Net.IPAddress.NetworkToHostOrder((short)_urgentPointer); set => _urgentPointer = (ushort)System.Net.IPAddress.HostToNetworkOrder((short)value); }

        // Options: (Variable 0–320 bits, divisible by 32)
        // Padding: The TCP header padding is used to ensure that the TCP header ends, and data begins, on a 32 bit boundary.
        //          The padding is composed of zeros.

        public static Tcp Create(ushort sourcePort, ushort destPort)
        {
            var tcp = new Tcp()
            {
                DestinationPort = destPort,
                SourcePort = sourcePort,
                ACK = true,
                CWR = false,
                DataOffset = (byte)(TcpHeaderWithOptions.SizeOfStandardHeader / 4),
                ECE = false,
                FIN = false,
                NS = false,
                PSH = false,
                RST = false,
                SYN = false,
                URG = false,
            };
            return tcp;
        }

        // Calculates the checksum in the Tcp Header. The pseudo header (minus the size) is always the same for the connection
        // So we only need to calculate for the size + Tcp Header and all data
        public void SetChecksum(ReadOnlySpan<byte> input, ulong pseudoHeaderSum)
        { 
            var size = (ushort)System.Net.IPAddress.HostToNetworkOrder((short)input.Length);
            pseudoHeaderSum = Network.Checksum.PartialCalculate(ref Unsafe.As<ushort, byte>(ref size), sizeof(ushort), pseudoHeaderSum);
            Checksum = Network.Checksum.Calculate(ref MemoryMarshal.GetReference(input), input.Length, pseudoHeaderSum);
        }

        public static bool TryConsume(ReadOnlySpan<byte> input, out Tcp tcp, out ReadOnlySpan<byte> options, out ReadOnlySpan<byte> data)
        {
            if (!TryConsume(input, out tcp, out data))
            {
                options = default;
                return false;
            }
            options = input.Slice(5 * 4);
            return true;
        }

        public static bool TryConsume(ReadOnlySpan<byte> input, out Tcp tcp, out ReadOnlySpan<byte> data)
        {
            if (input.Length >= Unsafe.SizeOf<Tcp>())
            {
                tcp = Unsafe.As<byte, Tcp>(ref MemoryMarshal.GetReference(input));
                // Checksum check
                data = input.Slice(tcp.DataOffset * 4, input.Length - (tcp.DataOffset * 4));
                return true;
            }

            tcp = default;
            data = default;
            return false;
        }

        public override string ToString() => "+- TCP Segment ------------------------------------------------------------------------+" + Environment.NewLine +
                  $"| :{SourcePort.ToString()} -> :{DestinationPort.ToString()} {(NS ? "NS " : "")}{(CWR ? "CWR " : "")}{(ECE ? "ECE " : "")}{(URG ? "URG " : "")}{(ACK ? "ACK " : "")}{(PSH ? "PSH " : "")}{(RST ? "RST " : "")}{(SYN ? "SYN " : "")}{(FIN ? "FIN " : "")}".PadRight(87) + "|";
    }
}
