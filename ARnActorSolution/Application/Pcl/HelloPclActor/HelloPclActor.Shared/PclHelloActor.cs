using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;
using System.Threading.Tasks;

namespace HelloPclActor
{
    public class PclHelloActor : BaseActor
    {
        private string fState;
        public PclHelloActor()
        {
            Become(new Behavior<string>(s =>
            {
                fState = s;
            }));
            AddBehavior(new Behavior<IActor>(a =>
            {
                a.SendMessage(fState);
            }));
        }

        public async Task<string> GetState()
        {
            IFuture<string> future = new Future<string>();
            this.SendMessage(future);
            return await future.ResultAsync();
        }
    }
}
