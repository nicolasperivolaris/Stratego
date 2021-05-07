using Server.Model;
using Stratego.Model;
using Stratego.Model.Panels;
using Stratego.Network;
using Stratego.Sockets.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Server.Server
{
    public class ServerController
    {
        private MainWindow View;
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
            View.Dispatcher.BeginInvoke((MethodInvoker)delegate ()
            {
                Players.Add(e.Player);
            });
        }
    }
}
