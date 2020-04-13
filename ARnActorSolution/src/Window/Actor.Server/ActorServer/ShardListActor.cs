using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{

    public class ShardListActor : ActionActor<string>
    {
        private readonly HashSet<string> fShardList = new HashSet<string>();

        public ShardListActor() : base() => AddBehavior(new Behavior<IFuture>(DoGetAll));

        public void Add(string aShardName) => SendAction(DoSend, aShardName);

        private void DoSend(string aShardName) => fShardList.Add(aShardName);

        public void Remove(string aShardName) => SendAction(DoRemove, aShardName);

        private void DoRemove(string aShardName) => fShardList.Remove(aShardName);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public Task<IEnumerable<string>> GetAllAsync()
        {
            IFuture<IEnumerable<string>> future = new Future<IEnumerable<string>>();
            SendMessage(future);
            return future.ResultAsync();
        }

        private void DoGetAll(IFuture future)
        {
            IEnumerable<string> list = fShardList.ToList().AsEnumerable();
            future.SendMessage(list);
        }
    }
}
