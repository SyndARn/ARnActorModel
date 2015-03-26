using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base ;
using Actor.Util;

namespace Actor.Neurone
{
    public class Synapse
    {
        public actNeurone fromNeurone { get; set; }
        public int Value{get ; set;}
        public int Activated { get; set; }
        public Synapse(actNeurone aNeurone, int aValue)
        {
            fromNeurone = aNeurone ;
            Value = aValue ;
            Activated = 0;
        }
    }
    
    public class actNeurone : actActor 
    {
        // dendrite receive from
        // Axone emit to
        // somma function
        // excitation level
        public List<actNeurone> Axone;
        public List<Synapse> Dendrite;
        public int fSeuil;
        public int fsum;

        public actNeurone()
            : base()
        {
            Dendrite = new List<Synapse>();
            Axone = new List<actNeurone>();
            fSeuil = 100;
            Become(new bhvBehavior<Tuple<actNeurone,int>>(ReceiveFromDendrite)) ;
            AddBehavior(new bhvBehavior<bool>(ReceiveFromInput));
        }

        private void ReceiveFromInput(bool aValue)
        {
            if (aValue)
            {
                Excited(12);
            }
        }

        private void ReceiveFromDendrite(Tuple<actNeurone,int> anActiver)
        {
            // find 
            Synapse sn = Dendrite.Where(t => t.fromNeurone.Equals(anActiver.Item1)).FirstOrDefault();
            sn.Activated = 1;
            Somma(anActiver.Item2);
        }

        private void Somma(int Decay)
        {
            fsum = Dendrite.Sum(t => t.Value * t.Activated);
            if (fsum > fSeuil)
            {
                Excited(Decay);
                foreach (var d in Dendrite)
                {
                    d.Activated = 0;
                }
                fsum = 0;
            }
            else // add value to dendrite
            {
                foreach(var d in Dendrite)
                {
                    if (d.Activated == 1)
                        d.Value = d.Value + 10;
                    else
                        d.Value = d.Value - 1;
                }
            }
        }

        private void Excited(int Decay)
        {
            if (Decay > 2)
            foreach (var neu in Axone)
            {
                SendMessageTo(Tuple.Create(this,Decay-1),neu);
            }
        }
    }

}
