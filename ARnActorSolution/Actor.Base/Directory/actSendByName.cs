using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    public class actSendByName<T> : actActor
    {
        T origMessage;
        public actSendByName()
        {
            Become(
                new bhvBehavior<Tuple<String, T>>(msg => { return msg is Tuple<string,T>; },
                    FindBehavior));
        }

        private void FindBehavior(Tuple<String, T> msg)
        {
            // find in directory
            origMessage = msg.Item2;
            Become(new bhvBehavior<Tuple<actDirectory.DirectoryRequest, IActor>>(ask => { return ask is Tuple<actDirectory.DirectoryRequest, IActor>; },
                SendBehavior));
            actDirectory.GetDirectory().Find(this, msg.Item1);
        }

        private void SendBehavior(Tuple<actDirectory.DirectoryRequest, IActor> ans)
        {
            if (ans.Item2 != null)
            {
                SendMessageTo(origMessage,ans.Item2);
            }
            Become(
                new bhvBehavior<Tuple<String, T>>(msg => { return msg is Tuple<string, T>; },
                    FindBehavior));
        }

        public static void SendByName(T aData, string anActor)
        {
            var act = new actSendByName<T>() ;
            act.SendMessageTo(Tuple.Create(anActor,
                aData), act);
        }
    }

}
