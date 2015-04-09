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
            AddBehavior(new bhvConsoleDictionary());
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
            Pattern = t => { return t is T; };
            Apply = DoConsole;
        }
        private void DoConsole(T msg)
        {
            Console.WriteLine(msg.ToString());
        }
    }

    public class bhvConsoleDictionary : bhvBehavior<Dictionary<string,string>>
    {
        public bhvConsoleDictionary()
            : base()
        {
            Pattern = t => t is Dictionary<string,string> ;
            Apply = DoConsole;
        }
        private void DoConsole(Dictionary<string,string> dico)
        {
            foreach (var item in dico)
            {
                Console.WriteLine(item.Key+" - "+item.Value);
            }
        }
    }

    public class bhvConsoleStringList : bhvBehavior<IEnumerable<String>>
    {
        public bhvConsoleStringList()
            : base()
        {
            Pattern = t => t is IEnumerable<String>; ;
            Apply = DoConsole;
        }
        private void DoConsole(IEnumerable<String> msg)
        {
            foreach (String s in msg)
            {
                Console.WriteLine(s);
            }
        }
    }

}
