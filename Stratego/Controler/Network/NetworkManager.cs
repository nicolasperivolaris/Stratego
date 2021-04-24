using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Stratego.Controler.Network
{
    public abstract class NetworkManager
    {
        public readonly int Port = 11000;

        public event EventHandler DataReceived;
        public event EventHandler PartnerArrival;

        public int DataSize { get; set; }

        public Socket ListeningSocket { get; set; }

        public NetworkManager(int dataSize)
        {
            DataSize = dataSize;
        }

        public abstract void Connect();

        public abstract void Send(String msg);

        protected void Send(Socket EndPoint, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            EndPoint.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), EndPoint);
        }

        protected void ReceiveDataAsync(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            int bytesRead;
            try
            {
                bytesRead = state.workSocket.EndReceive(ar);
            }
            catch (SocketException) { return; }

            DataReceived?.Invoke(((IPEndPoint)state.workSocket.RemoteEndPoint).Address,
                new StringEventArgs(Encoding.ASCII.GetString(state.buffer)));

            Array.Clear(state.buffer, 0, state.buffer.Length);
            state.workSocket.BeginReceive(state.buffer, 0, state.BufferSize, 0,
                    new AsyncCallback(ReceiveDataAsync), state);
        }

        protected virtual void SendCallback(IAsyncResult ar)
        {
            Console.WriteLine("Message send: " + ((Socket)ar.AsyncState).EndSend(ar));
        }

        public void OnPartnerArrival(Socket partner)
        {
            PartnerArrival?.Invoke(((IPEndPoint)partner.RemoteEndPoint).Address, null);
        }

        protected virtual void OnReceived(StringEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        public virtual void CloseConnection()
        {
            ListeningSocket.Close();
        }

        public abstract List<IPAddress> GetPartners();

        // State object for reading client data asynchronously  
        protected class StateObject
        {
            // Size of receive buffer.  
            public int BufferSize;

            // Receive buffer.  
            public byte[] buffer;

            // Client socket.
            public Socket workSocket = null;

            public StateObject(int bufferSize)
            {
                BufferSize = bufferSize;
                buffer = new byte[BufferSize];
            }
        }


    }
}
