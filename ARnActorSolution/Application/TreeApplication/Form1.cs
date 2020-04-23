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
using Actor.Windows;

namespace TreeApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        readonly StringToEventCatcherActor catcher = new StringToEventCatcherActor();

        protected void EvHandler(object sender, string i)
        {
            listBox1.Items.Add(i) ;
            catcher.SetEvent(listBox1, new EventHandler<string>(this.EvHandler));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int lMax = 8;
            int lAccu = 0;
            catcher.SetEvent(listBox1, new EventHandler<string>(this.EvHandler));
            PointActor pa = new PointActor();
            BaseActor accu = new BaseActor(new Behavior<int>(
                i =>
                {
                    lAccu++;
                    catcher.SendMessage(lAccu.ToString());
                }
                ));
            pa.SendMessage("AddLevel",0,lMax);
            pa.SendMessage("GiveLevel", (IActor)accu);
        }
    }
}
