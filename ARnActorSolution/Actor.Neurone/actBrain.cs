using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;

namespace Actor.Neurone
{

    public class actNeuroneLayer
    {
        public actNeurone[,]  fLayer ;
        public actNeuroneLayer(int row, int col)
        {
            fLayer = new actNeurone[row,col];
            for(int x =0;x<row;x++)
                for (int y = 0; y < col; y++)
                {
                    fLayer[x, y] = new actNeurone();
                }
        }
    }

    public class actBrain : BaseActor
    {
        actNeuroneLayer fInput;
        actNeuroneLayer fOutput;
        actNeuroneLayer fHidden;


        public void Start(string s)
        {
            SendMessage(s);
        }

        public void Result()
        {
            SendMessage(this);
        }


        private void DoOutput(IActor aSender)
        {
            string s = "";
            for(int x =0;x<26;x++)
                for (int y = 0; y < 8; y++)
                {
                    char c = ' ' ;
                    if (fOutput.fLayer[x, y].fsum > fOutput.fLayer[x, y].fSeuil)
                    {
                        c = (char)(x + (byte)'a');
                    }
                    s = s + c;
                }
            Console.WriteLine(s) ;
                //foreach (var d in neu.Dendrite)
                //{
                //    if (neu.)
                //    Console.WriteLine("    "+d.fromNeurone.Tag.Id + " " + d.Value + " "+d.Activated);
                //}
            // }
        }
        
        private void DoProcess(string s)
        {
            int i = 0;
            foreach (char c in s.ToLower().Where(c => c <= 'z' && c >= 'a'))
            {
                int v = (int)c - (int)'a' ;
                var neu = fInput.fLayer[v,i] ;
                neu.SendMessage(true) ;
                    if (i++ >= 7)
                        break ;
            }
        }

        public actBrain()
        {
            // init
            fInput = new actNeuroneLayer(26, 8);
            fOutput = new actNeuroneLayer(26, 8);
            fHidden = new actNeuroneLayer(26*4, 8*4);
            // link all input
            for(int x =0;x<26;x++)
                for (int y = 0; y < 8; y++)
                {
                    actNeurone neu = fInput.fLayer[x, y] ;
                    actNeurone lin = fHidden.fLayer[x,y] ;
                    neu.Axone.Add(lin) ;
                    lin.Dendrite.Add(new Synapse(neu, 100)) ;
                }
            // link all output
            for(int x =0;x<26;x++)
                for (int y = 0; y < 8; y++)
                {
                    actNeurone neu = fOutput.fLayer[x, y] ;
                    actNeurone lin = fHidden.fLayer[x*3+26,y*3+8] ;
                    lin.Axone.Add(neu) ;
                    neu.Dendrite.Add(new Synapse(lin, 100)) ;
                }
            // link intermediary
            // layer 1 to 2
            for (int x = 0; x < 26 ; x++)
                for (int y = 0; y < 8 ; y++)
                {
                    actNeurone neu = fHidden.fLayer[x, y];
                    int x1 = x +26 ;
                    int y1 = y +8;
                    actNeurone lin = fHidden.fLayer[x1, y1];
                    neu.Axone.Add(lin);
                    lin.Dendrite.Add(new Synapse(neu, 100));
                }
            // layer 2 to 3
            for (int x = 26; x < 26*2; x++)
                for (int y = 8; y < 8*2; y++)
                {
                    actNeurone neu = fHidden.fLayer[x, y];
                    int x1 = x + 26;
                    int y1 = y + 8;
                    actNeurone lin = fHidden.fLayer[x1, y1];
                    neu.Axone.Add(lin);
                    lin.Dendrite.Add(new Synapse(neu, 100));
                }
            // layer 3 to 4
            for (int x = 26*2; x < 26 * 3; x++)
                for (int y = 8*2; y < 8 * 3; y++)
                {
                    actNeurone neu = fHidden.fLayer[x, y];
                    int x1 = x + 26;
                    int y1 = y + 8;
                    actNeurone lin = fHidden.fLayer[x1, y1];
                    neu.Axone.Add(lin);
                    lin.Dendrite.Add(new Synapse(neu, 100));
                }

            // random
            Random rnd = new Random() ;
            for (int z = 0; z < 26*6*8*6; z++)
            {
                int x = rnd.Next(26 *4);
                int y = rnd.Next(8 * 4);
                        actNeurone neu = fHidden.fLayer[x, y];
                        int x1 = rnd.Next(26 * 4);
                        int y1 = rnd.Next(8 * 4);
                        int val = rnd.Next(1000);
                        actNeurone lin = fHidden.fLayer[x1, y1];
                        if (!neu.Equals(lin))
                        {
                            neu.Axone.Add(lin);
                            lin.Dendrite.Add(new Synapse(neu, val));
                        }

            }

            var bhv1 = new bhvBehavior<string>(t => {return t is string ; },DoProcess) ;
            var bhv2 = new bhvBehavior<IActor>(t => {return t is IActor ; },DoOutput) ;
            var bhv = new Behaviors();
            bhv.AddBehavior(bhv1);
            bhv.AddBehavior(bhv2);

            BecomeMany(bhv);
        }
    }
}
