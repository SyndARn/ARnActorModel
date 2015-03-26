using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    /// <summary>
    /// How an actor is known to other actor
    /// </summary>
    public interface IActor 
    {
        actTag Tag {get ;} 
    }

}
