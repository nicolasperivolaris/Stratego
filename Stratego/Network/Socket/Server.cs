using Stratego.Network;
using Stratego.Network.Socket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace Stratego.Sockets.Network
{
    public class Server : NetworkManager
    {
        public List<Socket> Clients { get; private set; }

        public Server() : base()
        {
            Clients = new List<Socket>();

            ListeningSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);
            ListeningSocket.Bind(localEndPoint);

            PartnerQuit += OnPartnerQuit;
        }

        private void OnPartnerQuit(object sender, IPAddressEventArgs e)
        {
            Clients.RemoveAll(c=>!c.Connected);
        }

        public override void Connect()
        {
            ListeningSocket.Listen(10);

            // Start an asynchronous socket to listen for connections.  
            Console.WriteLine("Waiting for a connection...");

            ListeningSocket.BeginAccept(
                new AsyncCallback(AcceptCallback),
                ListeningSocket);
        }

        protected void AcceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket client;
            try
            {
                client = listener.EndAccept(ar);
            }
            catch
            {
                return;
            }

            StateObject state = new StateObject()
            {
                WorkSocket = client
            };

            client.BeginReceive(state.Buffer, 0, state.BufferSize, 0,
                new AsyncCallback(ReceiveDataAsync), state);

            //client accepted
            Clients.Add(client);
            OnPartnerArrival(client);

            Send("Hello");

            //listen for more clients
            ListeningSocket.BeginAccept(
                new AsyncCallback(AcceptCallback),
                ListeningSocket);
        }

        public override void Send(string msg)
        {
            foreach (Socket client in Clients)
                Send(client, msg);
        }

        public override void CloseConnection()
        {
            base.CloseConnection();
            foreach (Socket client in Clients)
            {
                client.Close();
            }
        }

        public override void SendTo(string msg, List<Socket> partners)
        {
            foreach (Socket partner in partners)
            {
                SendTo(msg, partner);
            }
        }

        public override void SendTo(string msg, Socket partner)
        {
            Socket client = Clients.FirstOrDefault(c => (c == partner));
            if (client != null) Send(client, msg);
        }
    }
}
