using System;
using System.Threading;
using Magma.NetMap.Interop;
using Magma.Network.Abstractions;

namespace Magma.NetMap
{
    public sealed class NetMapReceiveRing<TPacketReceiver> : NetMapRing
        where TPacketReceiver : IPacketReceiver
    {
        private readonly Thread _worker;
        private TPacketReceiver _receiver;
        private NetMapTransmitRing _hostTxRing;
        private object _lock = new object();

        internal unsafe NetMapReceiveRing(RxTxPair rxTxPair, byte* memoryRegion, long queueOffset, TPacketReceiver receiver, NetMapTransmitRing hostTxRing)
            : base(rxTxPair, memoryRegion, queueOffset)
        {
            _hostTxRing = hostTxRing;
            _receiver = receiver;
            _worker = new Thread(new ThreadStart(ThreadLoop)) { IsBackground = true };
        }

        public void Start() => _worker.Start();

        private void ThreadLoop()
        {
            ref var ring = ref RingInfo();
            while (true)
            {
                while (!IsRingEmpty())
                {

                    var i = ring.Cursor;
                    ref var slot = ref GetSlot(i);
                    var buffer = _bufferPool.GetBuffer(slot.buf_idx);
                    buffer.RingId = this;
                    buffer.Length = slot.len;
                    ring.Cursor = RingNext(i);
                    if (!_receiver.TryConsume(buffer))
                    {
                        _hostTxRing.TrySendWithSwap(ref slot);
                        _hostTxRing.ForceFlush();
                    }
                    MoveHeadForward(slot.buf_idx);
                }
                _rxTxPair.WaitForWork();
            }
        }

        private void MoveHeadForward(uint bufferIndex)
        {
            lock(_lock)
            {
                ref var ring = ref RingInfo();
                ref var slot = ref GetSlot(ring.Head);
                if(slot.buf_idx != bufferIndex)
                {
                    slot.buf_idx = bufferIndex;
                    slot.flags |= Netmap.NetmapSlotFlags.NS_BUF_CHANGED;
                }
                ring.Head = RingNext(ring.Head);
            }
        }

        private void PrintSlotInfo(int index)
        {
            var slot = GetSlot(index);
            Console.WriteLine($"Slot {index} bufferIndex {slot.buf_idx} flags {slot.flags} length {slot.len} pointer {slot.ptr}");
        }
    }
}
