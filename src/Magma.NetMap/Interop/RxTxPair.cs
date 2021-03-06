using System;
using System.Text;
using static Magma.NetMap.Interop.Libc;
using static Magma.NetMap.Interop.Netmap;

namespace Magma.NetMap.Interop
{
    internal class RxTxPair:IDisposable
    {
        private FileDescriptor _rxFileDescriptor;
        private FileDescriptor _txFileDescriptor;
        private readonly int _ringId;
        private readonly bool _isHostStack;

        internal unsafe RxTxPair(string interfaceName, int ringId, bool isHostStack)
        {
            _ringId = ringId;
            _isHostStack = isHostStack;
            var flags = isHostStack ? NetMapRequestFlags.NR_REG_SW : NetMapRequestFlags.NR_REG_ONE_NIC;
            ringId = isHostStack ? 0 : ringId;
            
            _rxFileDescriptor = OpenNetMap(interfaceName, ringId, flags | NetMapRequestFlags.NR_RX_RINGS_ONLY | NetMapRequestFlags.NR_NO_TX_POLL, out var request);
            _txFileDescriptor = OpenNetMap(interfaceName, ringId, flags | NetMapRequestFlags.NR_TX_RINGS_ONLY, out request);
        }

        public void WaitForWork()
        {
            var pfd = new PollFileDescriptor()
            {
                Events = PollEvents.POLLIN,
                Fd = _rxFileDescriptor,
            };
            var result = Poll(ref pfd, 1, -1);
            if (result < 0) ExceptionHelper.ThrowInvalidOperation("Error on poll");
        }

        public unsafe void ForceFlush() => IOCtl(_txFileDescriptor, IOControlCommand.NIOCTXSYNC, IntPtr.Zero);

        public void Dispose()
        {
            if(_rxFileDescriptor.IsValid)
            {
                Close(_rxFileDescriptor);
                _rxFileDescriptor = new FileDescriptor();
            }
            if(_txFileDescriptor.IsValid)
            {
                Close(_txFileDescriptor);
                _txFileDescriptor = new FileDescriptor();
            }
        }
    }
}
