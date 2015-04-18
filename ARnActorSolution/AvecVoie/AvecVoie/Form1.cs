using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvecVoie
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private LifeBoard board;

        private void button1_Click(object sender, EventArgs e)
        {
            board = new LifeBoard(100);
            board.SendMessage("start");
        }
    }
}
