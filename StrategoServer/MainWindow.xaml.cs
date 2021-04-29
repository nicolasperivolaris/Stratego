using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
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
using Stratego;
using Stratego.Controler.Network;

namespace StrategoServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Server Server { get; set; }
        public List<Socket> Clients { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Server = new Server(500);
            Clients = Server.Clients;
        }
    }
}
