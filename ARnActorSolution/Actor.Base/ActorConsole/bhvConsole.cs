using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    public class bhvConsole : Behaviors
    {
        public bhvConsole()
            : base()
        {
            AddBehavior(new bhvConsole<string>());
            AddBehavior(new bhvConsoleStringList());
            AddBehavior(new bhvConsole<int>());
            AddBehavior(new bhvConsole<double>());
            AddBehavior(new bhvConsole<object>());
            //TODO add anything else if needed
        }
    }

    public class bhvConsole<T> : bhvBehavior<T>
    {
        public bhvConsole()
            : base()
        {
            this.Pattern = t => { return t is T; };
            this.Apply = DoConsole;
        }
        private void DoConsole(T msg)
        {
            Console.WriteLine(msg.ToString());
        }
    }

    public class bhvConsoleStringList : bhvBehavior<IEnumerable<String>>
    {
        public bhvConsoleStringList()
            : base()
        {
            this.Pattern = t => t is IEnumerable<String>; ;
            this.Apply = DoConsole;
        }
        private void DoConsole(IEnumerable<String> msg)
        {
            foreach(String s in msg)
              Console.WriteLine(s);
        }
    }

}
