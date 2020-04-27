using System;
using System.Windows.Forms;
using Actor.Base;
using Actor.Server;
using Actor.Service;
using Actor.Windows;

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
            ActorServer.Start(new ActorConfigManager());
            catcher = new StringToEventCatcherActor();
        }

        StringToEventCatcherActor catcher;

        protected void EvHandler(object sender, string message)
        {
            lblDuration.Text = message;
        }

        private void btStartStop_Click(object sender, EventArgs e)
        {
            int.TryParse(tbRingSize.Text, out int ringSize);
            int msgQtt = 0;
            int.TryParse(tbMessageQuantity.Text, out msgQtt);
            if ((ringSize > 0) && (msgQtt > 0))
            {
                catcher.SetEvent(lblDuration, new EventHandler<string>(this.EvHandler));
                new RingActor(ringSize, msgQtt, catcher);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTimeOffset start = DateTimeOffset.UtcNow;
            int qttMess = 0;
            int rcvmess = 0;
            int.TryParse(tbQtt.Text, out qttMess);
            if (qttMess > 0)
            {
                catcher.SetEvent(lblDuration, new EventHandler<string>(this.EvHandler));
                var actor = new BaseActor(new Behavior<string>(t => { return true; }, t =>
                {
                    rcvmess++; if (rcvmess >= qttMess)
                    {
                        DateTimeOffset end = DateTimeOffset.UtcNow;
                        catcher.SendMessage(string.Format("start {0} end {1} Duration{2}",
                            start.ToString(), end.ToString(), end.Subtract(start).ToString()));
                    }
                }
                ));
                for (int i = 0; i < qttMess; i++)
                {
                    actor.SendMessage("go");
                }
            }
        }

        private void tbRingSize_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
