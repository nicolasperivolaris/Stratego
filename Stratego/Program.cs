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
        public const int ENEMY = 1;

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
                Name = "Player",
                Color = Color.Green,
                Number = 0
            };
            players[Program.ENEMY] = new Player
            {
                Name = "Enemy",
                Color = Color.Red,
                Number = 1
            };
            GameControler controler = new GameControler(players);
            controler.Start();
        }
    }
}
