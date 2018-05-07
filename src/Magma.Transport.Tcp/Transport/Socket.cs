
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static Magma.Transport.Tcp.Transport.SocketConnection;

namespace Magma.Transport.Tcp.Transport
{
    public abstract class Socket : IDisposable
    {
        //public static async ValueTask<ListeningSocket> ListenAsync(IPEndPoint endPoint, int backlog, CancellationToken cancellationToken)
        //{
        //    var socket = new ListeningSocket(endPoint, backlog, cancellationToken);
        //    await socket.ListenAsync();
        //    return socket;
        //}

        //public static ValueTask<ConnectedSocket> ConnectAsync() => default;

        // IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Socket() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
    }
}