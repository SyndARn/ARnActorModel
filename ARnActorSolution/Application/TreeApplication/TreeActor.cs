using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base ;
using Actor.Util;

namespace TreeApplication
{
    class PointActor : BaseActor
    {
        private PointActor fLeft;
        private PointActor fRight;
        private int fLevel;
        private int fMax;
        public PointActor()
        {
            Become(new Behavior<string,int,int>(
                (s,i,m) => 
                {
                    fLevel = i;
                    fMax = m;
                    if (fLevel < m)
                    {
                        if (s == "AddLevel")
                        {
                            fLeft = new PointActor();
                            fRight = new PointActor();
                            fLeft.SendMessage(s, fLevel + 1, fMax);
                            fRight.SendMessage(s, fLevel + 1, fMax);
                        }
                    }
                })) ;
            AddBehavior(new Behavior<string, IActor>(
                (s,a) =>
                {
                    if (s == "GiveLevel")
                    {
                        a.SendMessage(fLevel);
                        if (fLeft != null)
                        {
                            fLeft.SendMessage(s, a);
                        }
                        if (fRight != null)
                        {
                            fRight.SendMessage(s, a);
                        }
                    }
                }
                ));
        }
    }
}
