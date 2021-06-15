using Stratego.Model;
using Stratego.Network;
using Stratego.Network.Socket;
using Stratego.Sockets.Network;
using Stratego.Utils;
using StrategoServer.Games;
using StrategoServer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Threading;

namespace StrategoServer.Server
{
    public class ServerController
    {
        private readonly MainWindow View;
        public ObservableCollection<Game> Games { get; set; }
        private NetworkController NetworkController { get; set; }

        public ServerController(MainWindow view)
        {
            View = view;
            NetworkController = new NetworkController();
            NetworkController.PlayerConnection += OnNewClient;
            NetworkController.PlayerLeave += OnPlayerLeave;
            NetworkController.Action += OnAction;
            NetworkController.Message += OnMessage;
            NetworkController.StartAsServer();

            Games = new ObservableCollection<Game>();
        }

        private void OnMessage(object sender, StringEventArgs e)
        {
            TransmitToOtherPlayers((Player)sender, e.Data);
        }

        private void TransmitToOtherPlayers(Player exception, Game game, object o)
        {
            List<Player> otherPlayers = game.Players.Where(p => !p.Equals(exception)).ToList();
            NetworkController.SendTo(otherPlayers, o);
        }

        private void TransmitToOtherPlayers(Player exception, object o)
        {
            Game game = Games.Single(g => g.Players.Contains(exception));
            TransmitToOtherPlayers(exception, game, o);
        }

        private void OnAction(object sender, ActionSerializer e)
        {
            TransmitToOtherPlayers((Player)sender, e);
        }

        private void OnPlayerLeave(object sender, PlayerEventArgs e)
        {
            Game gLeaving = Games.SingleOrDefault(g => g.Players.Contains(e.Player));
            View.Dispatcher.Invoke(delegate { gLeaving.Players.Remove(e.Player); });
            NetworkController.RemovePlayer(e.Player, gLeaving.Players.ToList());
        }

        private void OnNewClient(object sender, PlayerEventArgs e)
        {
            Game game = Games.FirstOrDefault(g=>!g.IsFull());
            if (game == null) //all the games are full
            {
                game = new Game(2, "Game" + (Games.Count));
                View.Dispatcher.Invoke((Action)delegate ()
                {
                    Games.Add(game);
                });
            }

            View.Dispatcher.Invoke(delegate { game.Add(e.Player); });


            TransmitToOtherPlayers(e.Player, game, e.Player); //introduce the new player to the other
            foreach (Player player in game.Players)
            {
                if(player != e.Player)
                {
                    NetworkController.SendTo(e.Player, player); //introduce other players to the new one
                }
            }
        }
    }
}
