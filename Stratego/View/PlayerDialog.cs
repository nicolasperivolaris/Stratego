using Stratego.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stratego.View
{
    public partial class PlayerDialog : Form
    {
        public PlayerDialog(Player player)
        {
            InitializeComponent();
            playerBindingSource.DataSource = player;
        }
    }
}
