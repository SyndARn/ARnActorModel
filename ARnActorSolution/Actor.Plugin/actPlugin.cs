using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Plugin
{
    public class actPlugin : BaseActor
    {
        public actPlugin()
            : base()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Console.WriteLine(asm.Location);
            Become(new Behavior<string>(Do));
        }

        public void Do(string msg)
        {
            // find real assembly
            Assembly asm = Assembly.GetExecutingAssembly();
            Console.WriteLine(msg + asm.Location);
        }
    }
}
