using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace Stratego.Controler.Network
{
    public class Server : NetworkManager
    {
        private List<Socket> Clients { get; set; }

        public Server(int dataSize) : base(dataSize)
        {
            Clients = new List<Socket>();

            ListeningSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);
            ListeningSocket.Bind(localEndPoint);
        }

        public void SendToAll(object serializable)
        {
            XmlSerializer serializer = new XmlSerializer(serializable.GetType());
            TextWriter tw = new StringWriter();
            serializer.Serialize(tw, serializable);

            foreach (Socket client in Clients)
                Send(client, tw.ToString());
        }

        public void SendToAll(String data)
        {
            foreach (Socket client in Clients)
                Send(client, data);
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

            StateObject state = new StateObject(DataSize)
            {
                workSocket = client
            };

            client.BeginReceive(state.buffer, 0, state.BufferSize, 0,
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
            {
                Send(client, msg);
            }
        }

        public override List<IPAddress> GetPartners()
        {
            List<IPAddress> list = new List<IPAddress>();
            foreach (Socket socket in Clients)
            {
                list.Add(((IPEndPoint)socket.RemoteEndPoint).Address);
            }
            return list;
        }

        public override void CloseConnection()
        {
            base.CloseConnection();
            foreach (Socket client in Clients)
            {
                client.Close();
            }
        }
    }
}
