using Actor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.TestApplication
{
    public class TestLauncherActor : ActionActor
    {
        private Future<bool> future = new Future<bool>();

        public TestLauncherActor()
            : base()
        {
        }

        public void Finish()
        {
            future.SendMessage(true);
        }

        public bool Wait()
        {
            return future.Result();
        }
    }
}
