using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    public abstract class PatternActor<T> : BaseActor
    {
        public PatternActor() : base()
        {
            Become(new Behavior<T>(DoApply));
        }

        protected abstract void DoApply(T msg);
    }

    public class StringPatternActor : PatternActor<string>
    {
        protected override void DoApply(string msg)
        {
            throw new NotImplementedException();
        }
    }

}
