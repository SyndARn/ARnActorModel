using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    enum ProxyCommand { RelayAsk, RelayFound } ;

    class actProxy : actActor
    {
        private IActor fLocalActor;
        private Uri fConnectedUri;

        public actProxy()
        {
            var remoteBehavior = new bhvBehavior<Tuple<Uri, Object>>(t => { return true; }, RemoteBehavior);
            var localBehavior = new bhvBehavior<Tuple<IActor, Object>>(t => { return true; }, LocalBehavior);
            var behaviors = new Behaviors();
            behaviors.AddBehavior(remoteBehavior);
            behaviors.AddBehavior(localBehavior);
            BecomeMany(behaviors);
        }

        // we receive from URI, we send to actor
        private void RemoteBehavior(Tuple<Uri, Object> msg)
        {
            SendMessageTo(msg.Item2,fLocalActor);
        }

        // we receive from actor, we send to URI
        private void LocalBehavior(Tuple<IActor, Object> msg)
        {
            // find host relay directory
            var lTask = Receive(t => { return t is Tuple<actDirectory.DirectoryRequest, IActor>; }).ContinueWith(
                t =>
                {
                    var ask = t.Result as Tuple<actDirectory.DirectoryRequest, IActor>;
                    if (ask.Item2 != null)
                    {
                        // find host relay 
                        SendMessageTo(Tuple.Create(fConnectedUri, "Find"),ask.Item2);
                        var lTaskFindHost =
                            Receive(t2 => { return t2 is Tuple<Uri, IActor>; }).ContinueWith(
                            t2 =>
                            {
                                var ask2 = t2.Result as Tuple<Uri, IActor>;
                                if (ask2.Item2 != null)
                                {
                                    // send to host relay
                                    SendMessageTo(Tuple.Create(msg.Item2, fConnectedUri,ask2.Item2));
                                }
                            });
                    }
                });
        }

    }
}
