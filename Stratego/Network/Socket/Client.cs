using Stratego.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Stratego.Sockets.Network
{
    public class Client : NetworkManager
    {
        private IPAddress ServerIP { get; set; }
        public Client(IPAddress server) : base()
        {
            ServerIP = server;
            // Create a TCP/IP socket.  
            ListeningSocket = new Socket(AddressFamily.InterNetwork,
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

                StateObject state = new StateObject()
                {
                    WorkSocket = client
                };
                client.BeginReceive(state.Buffer, 0, state.BufferSize, 0,
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
    }
}
