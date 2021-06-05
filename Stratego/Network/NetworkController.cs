using Stratego.Model;
using Stratego.Sockets.Network;
using Stratego.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;

namespace Stratego.Network
{
    public class NetworkController
    {
        public List<Player> Players { get; private set; }

        private NetworkManager NetworkManager;
        //private Dispatcher Dispatcher;

        public event EventHandler<PlayerEventArgs> PlayerConnection;
        public event EventHandler<StringEventArgs> Message;
        public event EventHandler<ActionEventArgs> Action;

        public NetworkController()
        {
            Players = new List<Player>();
        }

        /// <summary>
        /// Send an action on the network
        /// </summary>
        /// <param name="serializer"></param>
        public void Send(object o)
        {
            Flag f = Flag.Message;
            if (o is Player) f = Flag.Introducing;
            if (o is ActionSerializer) f = Flag.Action;
            if (o is String) f = Flag.Message;

            //Flag is the first char of a msg
            NetworkManager.Send((int)f + Serialize(o));
        }

        public void StopWaiting()
        {
            NetworkManager.CloseConnection();
        }

        public void StartAsServer()
        {
            Start(new Server());
        }

        public void StartAsClient(IPAddress server, Player client)
        {
            Start(new Client(server));
            Send(client);

        }

        private void Start(NetworkManager mode)
        {
            NetworkManager = mode;
            NetworkManager.Connect();
            NetworkManager.DataReceived += OnDataReceived;
        }

        private void OnDataReceived(object sender, StringEventArgs e)
        {
            IPAddress address = (IPAddress)sender;

            Flag f;
            String o;
            if (Enum.TryParse(e.Data[0] + "", out f) == false) 
            {
                f = Flag.Message;
                o = e.Data;
            }
            else
                o = e.Data.Substring(1); //first char is the flag

            Player player = Players.FirstOrDefault(p=>p.Address == address);
            switch (f)
            {
                case Flag.Introducing:
                    {
                        player = TryDeserialize<Player>(o);
                        player.Address = address;
                        if(!Players.Contains(player))Players.Add(player);
                        PlayerConnection?.Invoke(this, new PlayerEventArgs(player));
                        break; 
                    }
                case Flag.Action:
                    Action?.Invoke(this,TryDeserialize<ActionEventArgs>(o));
                    break;
                case Flag.Quit:
                    break;
                case Flag.Message:
                default:
                    Message?.Invoke(this, new StringEventArgs(o));
                    break;
            }
        }

        public static String Serialize(object o)
        {
            XmlSerializer serializer = new XmlSerializer(o.GetType());
            using (TextWriter tw = new StringWriter())
            {
                serializer.Serialize(tw, o);
                return tw.ToString();
            }
        }

        public static T TryDeserialize<T>(String data)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            try
            {
                using (TextReader reader = new StringReader(data))
                {
                    return (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception)
            {
                Console.WriteLine(data);
                return default;
            }
        }
    }
}
