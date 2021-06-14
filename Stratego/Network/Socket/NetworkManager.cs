using Stratego.Network.Socket;
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
        public event EventHandler<IPAddressEventArgs> PartnerArrival;
        public event EventHandler<IPAddressEventArgs> PartnerQuit;

        public const string EOL = "<EOL>";

        public Socket ListeningSocket { get; set; }

        public bool Connected { get; set; }

        public NetworkManager()
        {
            Connected = false;
        }

        public abstract void Connect();

        public abstract void Send(String msg);

        public abstract void SendTo(String msg, List<Socket> partners);

        public abstract void SendTo(String msg, Socket partners);

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


                String tmp = Encoding.ASCII.GetString(state.Buffer);
                if ((state.Content + tmp).Contains(EOL))
                {
                    string[] msgs = (state.Content + tmp).Trim('\0').Split(new string[] { EOL }, StringSplitOptions.None);

                    for (int i = 0; i < msgs.Length - 1; i++)//send all the messages but the queue if more than one in this stream 
                    {
                        DataReceived?.Invoke(state.WorkSocket,
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
            catch(SocketException)
            {
                PartnerQuit?.Invoke(this, new IPAddressEventArgs(state.address));
                Connected = false;
                return;
            }
        }

        protected virtual void SendCallback(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("Message send: " + ((Socket)ar.AsyncState).EndSend(ar));
            }
            catch (SocketException)
            {
                Console.Error.WriteLine("Connection failed");
            }
        }

        protected void OnPartnerArrival(Socket partner)
        {
            PartnerArrival?.Invoke(this, new IPAddressEventArgs(((IPEndPoint)partner.RemoteEndPoint).Address));
        }

        public virtual void CloseConnection()
        {
            ListeningSocket.Close();
        }

        // State object for reading client data asynchronously  
        protected class StateObject
        {
            // Size of receive buffer.  
            public int BufferSize = 100;

            // Receive buffer.  
            public byte[] Buffer;

            // Client socket.
            private Socket _workSocket = null;
            public Socket WorkSocket
            {
                get => _workSocket;
                set
                {
                    _workSocket = value;
                    if (value != null) address = ((IPEndPoint)_workSocket.RemoteEndPoint).Address;
                }
            }

            public IPAddress address;

            public StringBuilder Content;


            public StateObject()
            {
                Content = new StringBuilder();
                Buffer = new byte[BufferSize];
            }
        }


    }
}
