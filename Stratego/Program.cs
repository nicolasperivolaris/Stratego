using Stratego.Model;
using Stratego.Utils;
using Stratego.View;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Stratego
{
    static class Program
    {
        public const int PLAYER = 0;
        public const int ENEMI = 1;

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Player[] players = new Player[2];
            players[Program.PLAYER] = new Player
            {
                Name = "Player 1",
                Color = Color.Green,
                Number = 0,
                Address = System.Net.IPAddress.Loopback
            };
            GameControler controler = new GameControler(players);
            controler.Start();
        }
    }
}
