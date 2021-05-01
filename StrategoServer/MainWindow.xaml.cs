using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Stratego.Utils;
using Stratego.Model;
using StrategoServer.Games;
using Stratego.Sockets.Network;
using System.Collections.ObjectModel;
using Stratego.Network;
using System.Windows.Forms;

namespace StrategoServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Game> Games { get; set; }
        public ObservableCollection<Player> Players { get; set; }
        public NetworkController NetworkController { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            NetworkController = new NetworkController();
            NetworkController = new NetworkController();
            NetworkController.StartAsServer();
            NetworkController.PlayerConnection += OnNewClient;
            Players = new ObservableCollection<Player>(NetworkController.Players);
            Games = new ObservableCollection<Game>();
            gamesList.ItemsSource = Players;
        }

        public void OnGameListChange(object sender, EventArgs e)
        {

        }

        private void OnNewClient(object sender, PlayerEventArgs e)
        {
            Dispatcher.BeginInvoke((MethodInvoker)delegate ()
            {
                Players.Add(e.Player);
            });
        }
    }
}
