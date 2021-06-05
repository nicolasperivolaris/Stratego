using Stratego.Model;
using Stratego.Network;
using Stratego.Sockets.Network;
using Stratego.Utils;
using StrategoServer.Games;
using StrategoServer1;
using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace StrategoServer.Server
{
    public class ServerController
    {
        private readonly MainWindow View;
        public ObservableCollection<Game> Games { get; set; }
        private ObservableCollection<Player> Players { get; set; }
        private NetworkController NetworkController { get; set; }

        public ServerController(MainWindow view)
        {
            View = view;
            NetworkController = new NetworkController();
            NetworkController.PlayerConnection += OnNewClient;
            NetworkController.Action += OnAction;
            NetworkController.StartAsServer();

            Players = new ObservableCollection<Player>();
            Games = new ObservableCollection<Game>();
        }

        private void OnAction(object sender, ActionEventArgs e)
        {
            NetworkController.Send(e);
        }

        public void OnGameListChange(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnNewClient(object sender, PlayerEventArgs e)
        {
            View.Dispatcher.BeginInvoke((Action)delegate ()
            {
                Players.Add(e.Player);
                bool gameFound = false;
                foreach(Game g in Games)
                {
                    if (g.AcceptPlayers())
                    {
                        g.Players.Add(e.Player);
                        gameFound = true;
                        break;
                    }
                }
                if (!gameFound) Games.Add(new Game(2, "Game" + (Games.Count - 1)));
            });

        }
    }
}
