using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Actor.Base;

namespace LinqAndActorApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public delegate void AddText(string value);

        public class TextBoxActor : BaseActor
        {
            private AddText myDelegate;

            public TextBoxActor(TextBox textBox) : base()
            {
                myDelegate = new AddText(t => textBox.AppendText(t));
                Become(new Behavior<string>(
                    (str) => textBox.Invoke(myDelegate, str + Environment.NewLine)
                    ));
            }
        }

        private void LinqOperation_Click(object sender, EventArgs e)
        {
            var li = new LinqActor();
            li.SendMessage(tbTarget);
        }
    }
}
