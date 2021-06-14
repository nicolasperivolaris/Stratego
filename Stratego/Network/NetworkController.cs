using Stratego.Model;
using Stratego.Network.Socket;
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
        public event EventHandler<StringEventArgs> PlayerNumberReceived;
        public event EventHandler<PlayerEventArgs> PlayerLeave;
        public event EventHandler<GridEventArgs> GridReceived;
        public event EventHandler<StringEventArgs> Message;
        public event EventHandler<ActionSerializer> Action;
        public event EventHandler ConnectionError;

        public NetworkController()
        {
            Players = new List<Player>();
        }

        public NetworkController(Player[] players)
        {
            Players = players.ToList();
        }

        /// <summary>
        /// Send an object on the network to all other players
        /// </summary>
        /// <param name="serializer"></param>
        private void Send(object o)
        {
            Flag f = GetFlag(o);
            //Flag is the first char of a msg
            try
            {
                NetworkManager.Send(((int)f) + Serialize(o));
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                ConnectionError?.Invoke(this, EventArgs.Empty);
            }
        }

        private Flag GetFlag(object o)
        {
            Flag f = Flag.Crap;
            if (o is Player) f = Flag.IntroPlayer;
            else if (o is Grid) f = Flag.IntroGrid;
            else if (o is ActionSerializer) f = Flag.Action;
            else if (o is String) f = Flag.Message;
            return f;
        }

        public void SendTo(List<Player> players, object o)
        {
            Flag f = GetFlag(o);
            NetworkManager.SendTo((int)f + Serialize(o), players.Select(s => s.Socket).ToList());
        }

        public void SendTo(Player player, object o)
        {
            Flag f = GetFlag(o);
            NetworkManager.SendTo((int)f + Serialize(o), player.Socket);
        }

        public void RemovePlayer(Player player, List<Player> partners)
        {
            Players.Remove(player);//if not already done
            NetworkManager.SendTo((int)Flag.Quit + Serialize(player), partners.Select(s=>s.Socket).ToList());
        }

        public void Send(ActionSerializer action)
        {
            Send((object)action);
        }

        public void Send(String s)
        {
            Send((object)s);
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
            NetworkManager.PartnerQuit += OnPartnerQuit;
            NetworkManager.PartnerArrival += OnPartnerArrival;
        }

        public bool IsConnected() 
        {
            return NetworkManager == null ? false : NetworkManager.Connected;
        }
        public bool IsDisconnected()
        {
            return !NetworkManager.Connected;
        }

        private void OnPartnerArrival(object sender, IPAddressEventArgs e)
        {
            Console.WriteLine("New player");
        }

        private void OnPartnerQuit(object sender, IPAddressEventArgs e)
        {
            Player p = Players.SingleOrDefault(s => s.Address == e.Address);
            PlayerLeave?.Invoke(this, new PlayerEventArgs(p));
            Players.Remove(p);
        }

        private void OnDataReceived(object sender, StringEventArgs e)
        {
            System.Net.Sockets.Socket socket = (System.Net.Sockets.Socket)sender;

            Flag f;
            String o;
            if (Enum.TryParse(e.Data[0] + "", out f) == false) 
            {
                f = Flag.Crap;
                o = e.Data;
            }
            else
                o = e.Data.Substring(1); //first char is the flag

            Player player = Players.FirstOrDefault(p=>p.Socket == socket);
            switch (f)
            {
                case Flag.IntroPlayer:
                    {
                        player = TryDeserialize<Player>(o);
                        player.Socket = socket;
                        if (NetworkManager is Server && !Players.Contains(player))
                        {
                            Players.Add(player);
                            player.Number = Players.Count;
                            NetworkManager.SendTo((int)Flag.PlayerNumber + "" +player.Number, player.Socket);
                        }else if(NetworkManager is Client)
                        {
                            Players[Program.ENEMY].Name = player.Name;
                            Players[Program.ENEMY].Socket = player.Socket;
                            Players[Program.ENEMY].Number = player.Number;
                        }
                        PlayerConnection?.Invoke(player, new PlayerEventArgs(player));
                        break; 
                    }
                case Flag.PlayerNumber:
                    {
                        PlayerNumberReceived?.Invoke(player, new StringEventArgs(o));
                        break;
                    }
                case Flag.IntroGrid:
                    {
                        break;
                    }
                case Flag.Action:
                    ActionSerializer action = TryDeserialize<ActionSerializer>(o);
                    action.Player = player;
                    Action?.Invoke(player, action);
                    break;
                case Flag.Quit: PlayerLeave?.Invoke(player, new PlayerEventArgs(player));
                    break;
                case Flag.Message:
                    Message?.Invoke(player, new StringEventArgs(o));
                    break;
                default:
                    Console.WriteLine(o);
                    break;
            }
        }

        public static String Serialize(object o)
        {
            if (o is String)
                return (String)o;

            XmlSerializer serializer = new XmlSerializer(o.GetType());
            using (TextWriter tw = new StringWriter())
            {
                serializer.Serialize(tw, o);
                return tw.ToString();
            }
        }

        public void BreakConnection()
        {
            NetworkManager.Send((int)Flag.Quit + "" + Flag.Quit);
            NetworkManager.CloseConnection();
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
