using System;
using System.IO;
using System.Runtime.CompilerServices;
using Magma.Internet.Icmp;
using Magma.Internet.Ip;
using Magma.Network;
using Magma.Network.Abstractions;
using Magma.Network.Header;

namespace Magma.NetMap.Host
{
    class PacketReceiver : IPacketReceiver
    {
        private int _ringId;
        private NetMapTransmitRing _transmitter;
        private TextWriter _streamWriter;

        public PacketReceiver(int ringId, NetMapTransmitRing transmitter, bool logToFile)
        {
            _transmitter = transmitter;
            var filename = Path.Combine(Directory.GetCurrentDirectory(), $"rxOutput{ringId}.txt");
            Console.WriteLine($"Outputing recieved packets to: {filename}");
            _streamWriter = logToFile ? new StreamWriter(filename) : null;
            _ringId = ringId;
        }
        
        public bool TryConsume(int ringId, Span<byte> buffer)
        {
            WriteLine($"span Length: {buffer.Length.ToString()}");
            if (Ethernet.TryConsume(ref buffer, out var ethernet))
            {
                WriteLine($"{ethernet.ToString()}");
                WriteLine($".span Length: {buffer.Length.ToString()}");

                if (ethernet.Ethertype == EtherType.IPv4)
                {
                    if (IPv4.TryConsume(ref buffer, out var ip))
                    {
                        WriteLine($"{ip.ToString()}");
                        WriteLine($"..span Length: {buffer.Length.ToString()}");

                        var protocol = ip.Protocol;
                        if (protocol == ProtocolNumber.Tcp)
                        {
                            if (Tcp.TryConsume(ref buffer, out var tcp))
                            {
                                WriteLine($"{tcp.ToString()}");
                                WriteLine($"...span Length: {buffer.Length.ToString()}");
                            }
                        }
                        else if (protocol == ProtocolNumber.Icmp)
                        {
                            if (IcmpV4.TryConsume(ref buffer, out var icmp))
                            {
                                WriteLine($"{icmp.ToString()}");

                                if (icmp.Code == Code.EchoRequest)
                                {
                                    // Need to transmit here...
                                }
                            }
                        }
                    }
                }
                else
                {
                    WriteLine($"{ ethernet.Ethertype.ToString().PadRight(11)} ---> {BitConverter.ToString(buffer.ToArray()).Substring(60)}...");
                }
                WriteLine("+--------------------------------------------------------------------------------------+" + Environment.NewLine);
            }
            else
            {
                WriteLine($"Unknown ---> {BitConverter.ToString(buffer.ToArray()).Substring(60)}...");
            }
            
            Flush();
            return false;
        }

        private void WriteLine(string output) => _streamWriter?.WriteLine(output);
        private void Flush() => _streamWriter?.Flush();
    }
    
    class Program
    {
        private static int RingId = 0;

        static unsafe void Main(string[] args)
        {
            var interfaceName = "eth0";
            if (args.Length >= 1)
            {
                interfaceName = args[0];
            }

            Console.WriteLine($"Ethernet Header length: {Unsafe.SizeOf<Ethernet>()}");
            Console.WriteLine($"IP Header length: {Unsafe.SizeOf<IPv4>()}");
            Console.WriteLine($"TCP Header length: {Unsafe.SizeOf<Tcp>()}");

            var netmap = new NetMapPort<PacketReceiver>(interfaceName, transmitter => new PacketReceiver(RingId++, transmitter, logToFile : true));
            netmap.Open();
            netmap.PrintPortInfo();

            Console.WriteLine("Started reading");
            Console.ReadLine();
        }
    }
}
