using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Actor.Base;

namespace WebQuote
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var future = new Future<string>();
            var yahoo = new YahooQuote(tbCCY1.Text+tbCCY2.Text,future);
            tbQuote.Text = await future.ResultAsync();
        }
    }
}
