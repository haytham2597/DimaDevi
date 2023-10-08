using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DimaDevi.Libs;

namespace DimaDevi.Modules
{
    public class UDPSocket
    {
        public class ReceiveReachedEventArgs : EventArgs
        {
            public string Result { set; get; }
            public bool IsClient { set; get; }
        }
        public event EventHandler<ReceiveReachedEventArgs> OnReceive;
        protected virtual void OnReceiveChanged(ReceiveReachedEventArgs e)
        {
            EventHandler<ReceiveReachedEventArgs> handler = OnReceive;
            handler?.Invoke(this, e);
        }

        private bool isClient;
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public void Server(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Receive();
        }

        public void Client(string address, int port)
        {
            _socket.Connect(IPAddress.Parse(address), port);
            isClient = true;
            Receive();
        }

        public void Send(string text)
        {
            byte[] data = DeviGeneralConfig.GetInstance().Encoding.GetBytes(text);
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
#if DEBUG
                Console.WriteLine("SEND: {0}, {1}", bytes, text);
#endif
            }, state);
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);

                ReceiveReachedEventArgs args = new ReceiveReachedEventArgs
                {
                    Result = DeviGeneralConfig.GetInstance().Encoding.GetString(so.buffer, 0, bytes),
                    IsClient = isClient
                };
                OnReceiveChanged(args);
#if DEBUG
                Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
#endif
            }, state);
        }
    }
}
