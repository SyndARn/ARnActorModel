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
using Actor.Util;


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
                myDelegate = new AddText(t => textBox.AppendText((t)));
                Become(new Behavior<string>(
                    (str) =>
                    {
                        textBox.Invoke(myDelegate, str+Environment.NewLine);
                    }
                    ));
            }

        }

        public class linqActor : BaseActor
        {
            public linqActor() : base()
            {
                Become(new Behavior<TextBox>(DoIt));
            }
            private void DoIt(TextBox tb)
            {
                var col = new EnumerableActor<Tuple<int, int>>();
                var tbActor = new TextBoxActor(tb);

                // fill our collection
                for (int i = 0; i <= 100000; i++)
                {
                    col.Add(Tuple.Create(i % 100, i));
                }

                tbActor.SendMessage(col.Count.ToString());
                // apply some linq things
                var map = from item in col
                          select Tuple.Create(item.Item1.ToString(), item.Item2);
                // regroup 
                var regroup = from item in map
                              group item by item.Item1 into g
                              select g;
                // reduce
                var reduce = from item in regroup
                             select Tuple.Create(item.Key, item.Sum(t => t.Item2));

                // reduce 2
                var reduce2 = reduce.AsActorQueryiable();

                // project
                foreach (var item in reduce2)
                    tbActor.SendMessage(item.Item1 + " - " + item.Item2.ToString());

            }
        }

        private void LinqOperation_Click(object sender, EventArgs e)
        {
            var li = new linqActor();
            li.SendMessage(tbTarget);

        }
    }
}
