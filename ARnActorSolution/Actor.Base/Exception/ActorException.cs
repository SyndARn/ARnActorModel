using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    [Serializable]
    public class ActorException : Exception
    {
        public ActorException()
            : base()
        { }

        public ActorException(string message)
            : base(message)
        { }
    }
    // CA1032	Implement standard exception constructors	Add the following constructor to 'ActorException': public ActorException(String, Exception).	Actor.Base	ActorException.cs	10

}
