using Actor.Base;
using Actor.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ActorRing
{
    public partial class frmRing : Form
    {

        public frmRing()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ActorServer.Start("localhost", 80,false);
            catcher = new actStringCatcher();
        }

        actStringCatcher catcher;

        protected void EvHandler(object sender,string e)
        {
            lblDuration.Text = e;
        }

        private void btStartStop_Click(object sender, EventArgs e)
        {
            int ringSize = 0 ;
            int.TryParse(tbRingSize.Text,out ringSize) ;
            int msgQtt = 0 ;
            int.TryParse(tbMessageQuantity.Text,out msgQtt) ;
            if ((ringSize >0) && (msgQtt >0))
            {
                catcher.SetEvent(lblDuration, new EventHandler<string>(this.EvHandler));
                new actRing(ringSize, msgQtt, catcher);
            }
        }
    }
}
