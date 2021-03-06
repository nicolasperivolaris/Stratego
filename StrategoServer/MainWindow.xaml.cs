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
using Stratego.Model.Panels;
using StrategoServer.Server;
using System.Windows.Controls;

namespace StrategoServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ServerController ServerController;
        public MainWindow()
        {
            InitializeComponent();
            ServerController = new ServerController(this);
            gamesList.ItemsSource = ServerController.Games;
        }

        private void gamesList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DataContext = (Game)((ListBox)sender).SelectedItem;
        }
    }
}
