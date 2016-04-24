using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base;
using Actor.Util;

namespace Actor.Util
{
    public class MapReduceActor<D,TKeyMap, TValueMap, TKeyReduce, TValueReduce> : BaseActor
    {

        private Dictionary<TKeyReduce, List<TValueReduce>> fDict = new Dictionary<TKeyReduce, List<TValueReduce>>();

        private int fActiveMap;

        public MapReduceActor
            (
                // Func<D, IEnumerable<Tuple<Km, Vm>>> parserKV,
                Action<IActor,D> senderKV,
                Action<IActor, TKeyMap, TValueMap> mapKV,
                Func<TKeyReduce, IEnumerable<TValueReduce>, TValueReduce> reduceKV,
                IActor outputActor
            ) : base()
        {
            // start reduce
            var bhvStart = new Behavior<D>(d =>
            {
                fActiveMap++;
                senderKV(this, d);
            });

            // receive data to process
            var bhvInput = new Behavior<TKeyMap,TValueMap>(
                (k,v) =>
                {
                    // parse data
                        var map = new MapActor<TKeyMap, TValueMap>(this, mapKV);
                        fActiveMap++;
                        map.SendMessage((IActor)this, k, v);
                }
                );

            // end parse
            var bhvEndParse = new Behavior<D, IActor>((d, a) =>
             {
                 fActiveMap--;
             });

            // receive from Map, index
            var bhvMap2Index = new Behavior<TKeyReduce, TValueReduce>
                (
                (k, v) =>
                {
                    List<TValueReduce> someValue;
                    if (!fDict.TryGetValue(k, out someValue))
                    {
                        fDict[k] = new List<TValueReduce>();
                    }
                    fDict[k].Add(v);
                }
                );

            // receive end of job from Map, go to reduce
            var bhvMap2EndOfJov = new Behavior<MapActor<TKeyMap, TValueMap>>
                (
                (a) =>
                {
                    fActiveMap--;
                    if (fActiveMap <= 0)
                    {
                        // launch reduce
                        foreach (var item in fDict)
                        {
                            var red = new ReduceActor<TKeyReduce, TValueReduce>(this, reduceKV);
                            red.SendMessage(item.Key, item.Value.AsEnumerable());
                        }
                    }
                }
                );

            // receive from Reduce, send to output
            var bhvReduceToOutput = new Behavior<ReduceActor<TKeyReduce, TValueReduce>, TKeyReduce, TValueReduce>
                (
                (r, k, v) =>
                {
                    outputActor.SendMessage(k, v);
                }
                );


            Behaviors bhvs = new Behaviors();
            bhvs.AddBehavior(bhvStart);
            bhvs.AddBehavior(bhvEndParse);
            bhvs.AddBehavior(bhvInput);
            bhvs.AddBehavior(bhvMap2Index);
            bhvs.AddBehavior(bhvMap2EndOfJov);
            bhvs.AddBehavior(bhvReduceToOutput);
            Become(bhvs);
        }
    }

    public class MapActor<Km, Vm> : BaseActor
    {
        IActor fSender;
        Action<IActor, Km, Vm> fMapAction;
        public MapActor(IActor sender, Action<IActor, Km, Vm> mapAction) : base()
        {
            fSender = sender;
            fMapAction = mapAction;
            Become(new Behavior<IActor, Km, Vm>(DoMapAction));
        }

        private void DoMapAction(IActor act, Km key, Vm value)
        {
            fMapAction(fSender, key, value);
            // end of job
            fSender.SendMessage(this);
        }
    }

    public class ReduceActor<K, V> : BaseActor
    {
        IActor fSender;
        Func<K, IEnumerable<V>, V> fReduceAction;

        public ReduceActor(IActor sender, Func<K, IEnumerable<V>, V> reduceAction) : base()
        {
            fReduceAction = reduceAction;
            fSender = sender;
            Become(new Behavior<K, IEnumerable<V>>(DoReduceAction));
        }

        private void DoReduceAction(K key, IEnumerable<V> someValues)
        {
            V value = fReduceAction(key, someValues);
            fSender.SendMessage(this, key, value);
        }
    }



}
