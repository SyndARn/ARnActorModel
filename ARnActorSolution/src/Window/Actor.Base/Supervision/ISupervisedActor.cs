using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{
    public interface ISupervisedActor : IActor
    {
        ISupervisedActor Respawn();
    }
}
