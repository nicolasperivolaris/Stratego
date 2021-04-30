using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Stratego.Utils;
using Stratego.Model;
using StrategoServer.Games;
using Stratego.Sockets.Network;

namespace StrategoServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Server Server { get; set; }
        public List<Game> Games { get; set; }
        public List<Player> Players { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Server = new Server();
            Server.PartnerArrival += OnNewClient;
            Server.DataReceived += OnDataReceived;
            DataContext = Games;
        }

        private void OnDataReceived(object sender, EventArgs e)
        {
            if (e is StringEventArgs args)
            {
                Enum.TryParse(args.Data.Split('\n')[0], out Flag flag);
                switch (flag)
                {
                    case Flag.Introducing:
                        Players.First(p => p.Address == (IPAddress)sender);
                        break;
                    case Flag.Map:
                        break;
                    case Flag.Action:
                        break;
                    case Flag.Quit:
                        break;
                    case Flag.Message:
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnNewClient(object sender, EventArgs e)
        {
            Player player = new Player()
            {
                Address = (IPAddress)sender
            };
            Players.Add(player);
        }
    }
}
