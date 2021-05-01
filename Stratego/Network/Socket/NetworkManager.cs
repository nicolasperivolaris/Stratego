using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Stratego.Sockets.Network
{
    public abstract class NetworkManager
    {
        public readonly int Port = 5500;

        public event EventHandler<StringEventArgs> DataReceived;
        public event EventHandler PartnerArrival;

        public const string EOL = "<EOL>";

        public Socket ListeningSocket { get; set; }

        public abstract void Connect();

        public abstract void Send(String msg);

        protected void Send(Socket EndPoint, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data + EOL);

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
                bytesRead = state.WorkSocket.EndReceive(ar);
            }
            catch (SocketException) { return; }

            String tmp = Encoding.ASCII.GetString(state.Buffer);
            if ((state.Content + tmp).Contains(EOL))
            {            
                string[] msgs = (state.Content + tmp).Split(new string[] { EOL }, StringSplitOptions.None);

                for (int i = 0; i < msgs.Length - 1; i++)//send all the messages but the queue if more than one in this stream 
                {
                    DataReceived?.Invoke(((IPEndPoint)state.WorkSocket.RemoteEndPoint).Address,
                    new StringEventArgs(msgs[i]));
                }
                state.Content.Clear();
                state.Content.Append(msgs[msgs.Length - 1]); // save the queue
            }
            else
            {
                state.Content.Append(tmp);
            }


            Array.Clear(state.Buffer, 0, state.BufferSize);
            state.WorkSocket.BeginReceive(state.Buffer, 0, state.BufferSize, 0,
                    new AsyncCallback(ReceiveDataAsync), state);
        }

        protected virtual void SendCallback(IAsyncResult ar)
        {
            Console.WriteLine("Message send: " + ((Socket)ar.AsyncState).EndSend(ar));
        }

        protected void OnPartnerArrival(Socket partner)
        {
            PartnerArrival?.Invoke(((IPEndPoint)partner.RemoteEndPoint).Address, EventArgs.Empty);
        }

        protected virtual void OnReceived(StringEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        public virtual void CloseConnection()
        {
            ListeningSocket.Close();
        }

        // State object for reading client data asynchronously  
        protected class StateObject
        {
            // Size of receive buffer.  
            public int BufferSize = 2;

            // Receive buffer.  
            public byte[] Buffer;

            // Client socket.
            public Socket WorkSocket = null;

            public StringBuilder Content;

            public StateObject()
            {
                Content = new StringBuilder(); 
                Buffer = new byte[BufferSize];
            }
        }


    }
}
