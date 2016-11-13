using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;
using System.Windows.Forms;
using System.IO;

namespace MapReduceApplication
{
    public class TextBoxActor : BaseActor
    {
        public delegate void AddText(string aString);
        public AddText myDelegate;

        public TextBoxActor(TextBox textBox) : base()
        {
            myDelegate = new AddText(t => textBox.AppendText((t)));
            Become(new Behavior<string, int>(
                (str, i) =>
                {
                    string s = str + "-:-" + i.ToString() + Environment.NewLine;
                    textBox.Invoke(myDelegate, s);
                }
                ));
        }

    }

    public class MapReduceTest
    {
        public MapReduceTest()
        {

        }

        public void Go(string aFilename, TextBox textBox)
        {
            var sb = new TextBoxActor(textBox);
            var mapReduce = new MapReduceActor<string, string, string, string, int>
                (
                // parse
                (a, d) =>
                {
                    using (StreamReader sr = new StreamReader(d))
                    {
                        while (!sr.EndOfStream)
                        {
                            string s = sr.ReadLine();
                            a.SendMessage(d, s);
                        }
                    }
                    a.SendMessage(d, a);
                },
                // map
                (a, k, v) =>
                {
                    string[] stab = v.Split(' ');
                    foreach (var item in stab)
                    {
                        a.SendMessage(item, 1);
                    }

                },
                // reduce
                (k, v) =>
                {
                    int sum = v.Count();
                    return sum;
                },
                // output
                sb);
            mapReduce.SendMessage(aFilename);
        }
    }
}
