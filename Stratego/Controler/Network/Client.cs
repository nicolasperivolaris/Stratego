using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Stratego.Controler.Network
{
    public class Client : NetworkManager
    {
        private IPAddress ServerIP { get; set; }
        public Client(IPAddress server, int dataSize) : base(dataSize)
        {
            //ServerIP = Dns.GetHostAddresses("localhost")[0];
            // Establish the remote endpoint for the socket.  
            //IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            ServerIP = server;
            // Create a TCP/IP socket.  
            ListeningSocket = new Socket(ServerIP.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
        }

        public override void Connect()
        {
            // Connect to the remote endpoint.  
            ListeningSocket.BeginConnect(new IPEndPoint(ServerIP, Port),
                new AsyncCallback(ConnectCallback), ListeningSocket);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Client connected to {0}",
                    client.RemoteEndPoint.ToString());

                StateObject state = new StateObject(DataSize)
                {
                    workSocket = client
                };
                client.BeginReceive(state.buffer, 0, state.BufferSize, 0,
                new AsyncCallback(ReceiveDataAsync), state);

                Send("Hello");

                OnPartnerArrival(client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public override void Send(string msg)
        {
            Send(ListeningSocket, msg);
        }

        public override List<IPAddress> GetPartners()
        {
            List<IPAddress> list = new List<IPAddress>();
            list.Add(ServerIP);
            return list;
        }
    }
}
