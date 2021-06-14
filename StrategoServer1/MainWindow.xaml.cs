using Stratego.Model;
using Stratego.Model.Tiles;
using StrategoServer.Games;
using StrategoServer.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StrategoServer1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        private readonly ServerController ServerController;
        private Label[] LabelsGrid;
        public MainWindow()
        {
            InitializeComponent();
            ServerController = new ServerController(this);
            gamesList.ItemsSource = ServerController.Games;
        }

        private void GamesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Game game = (Game)((ListBox)sender).SelectedItem;
            DataContext = game;
            playersList.ItemsSource = game.Players;
            //SetGrid(game.Grid);
        }

        private void SetGrid(Tile[] grid)
        {
            LabelsGrid = new Label[grid.Length];
            int i = 0;
            foreach (Tile tile in grid)
            {
                Label label = new Label();
                if (tile.Accessible)
                {
                    label.Background = new SolidColorBrush(Colors.Yellow);
                }
                else
                {
                    label.Background = new SolidColorBrush(Colors.Blue);
                }
                this.grid.Children.Add(label);
                LabelsGrid[i] = label;
                i++;
            }
        }
    }
}
