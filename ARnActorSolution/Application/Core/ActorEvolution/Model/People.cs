using System;
using System.Collections.Generic;
using System.Text;
using Actor.Base;

namespace ActorEvolution.Model
{
    public class People
    {
        public Position Position { get; } = new Position();
        public string Name { get; set; }
        public string Job { get; set; }
        public long Money { get; set; }
        public long Eat { get; set; }
        public long Age { get; set; }
        public long Health { get; set; }
    }

    public class Position
    {
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }
        public long T { get; set; }
    }
}
